using MTV3D65;
using SWEndor.Actors;
using SWEndor.Primitives;

namespace SWEndor.AI.Actions
{
  public class ProjectileAttackActor : ActionInfo
  {
    public ProjectileAttackActor(ActorInfo targetActor) : base("ProjectileAttackActor")
    {
      Target_Actor = targetActor;
      ID = targetActor?.ID ?? -1;
      CanInterrupt = false;
    }

    // parameters
    public readonly ActorInfo Target_Actor = null;
    public readonly int ID;
    public TV_3DVECTOR Target_Position = new TV_3DVECTOR();

    public override string ToString()
    {
      return "{0},{1},{2},{3}".F(Name
                              , ID
                              , CanInterrupt
                              , Complete
                              );
    }

    public override void Process(Engine engine, ActorInfo actor)
    {
      ActorInfo target = Target_Actor;
      if (target == null || ID != target.ID)
      {
        Complete = true;
        return;
      }

      float dist = ActorDistanceInfo.GetDistance(actor, Target_Actor);
      float d = dist / Globals.LaserSpeed;

      ActorInfo a2 = target.UseParentCoords ? target.Parent : null;
      if (a2 == null)
        Target_Position = target.GetRelativePositionXYZ(0, 0, target.MoveData.Speed * d);
      else
        Target_Position = target.GetRelativePositionXYZ(0, 0, a2.MoveData.Speed * d);

      AdjustRotation(actor, Target_Position, true);
      AdjustSpeed(actor, actor.MoveData.MaxSpeed);

      Complete |= (!target.Active || target.IsDyingOrDead);
    }
  }
}
