using MTV3D65;
using SWEndor.Actors;
using SWEndor.Core;
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
      return string.Join(",", new string[]
      {
          Name
        , ID.ToString()
        , CanInterrupt.ToString()
        , Complete.ToString()
      });
    }

    public override void Process(Engine engine, ActorInfo actor)
    {
      ActorInfo target = Target_Actor;
      if (target == null || ID != target.ID)
      {
        Complete = true;
        return;
      }

      actor.AIData.SetTarget(engine, actor, target, true);
      actor.AIData.AdjustRotation(engine, actor);
      actor.AIData.SetTargetSpeed(actor.MoveData.MaxSpeed);
      actor.AIData.AdjustSpeed(actor);

      Complete |= (!target.Active || target.IsDyingOrDead);
    }
  }
}
