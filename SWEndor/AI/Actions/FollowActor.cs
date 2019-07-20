using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Actors.Data;
using SWEndor.Primitives;

namespace SWEndor.AI.Actions
{
  public class FollowActor : ActionInfo
  {
    public FollowActor(ActorInfo target, float follow_distance = 500, bool can_interrupt = true) : base("FollowActor")
    {
      Target_Actor = target;
      FollowDistance = follow_distance;
      CanInterrupt = can_interrupt;
    }

    // parameters
    public ActorInfo Target_Actor = null;
    public float FollowDistance = 500;
    public float SpeedAdjustmentDistanceRange = 100;

    public override string ToString()
    {
      return "{0},{1},{2},{3},{4},{5}".F(Name
                                      , Target_Actor.ID
                                      , FollowDistance
                                      , SpeedAdjustmentDistanceRange
                                      , CanInterrupt
                                      , Complete
                                      );
    }

    public override void Process(Engine engine, ActorInfo actor)
    {
      ActorInfo target = Target_Actor;
      if (target == null || actor.MoveData.MaxSpeed == 0)
      {
        Complete = true;
        return;
      }

      if (CheckBounds(actor))
      {
        AdjustRotation(actor, target.GetGlobalPosition());
        float dist = ActorDistanceInfo.GetDistance(actor, target, FollowDistance + 1);

        float addspd = (actor.MoveData.MaxSpeed > target.MoveData.Speed) ? actor.MoveData.MaxSpeed - target.MoveData.Speed : 0;
        float subspd = (actor.MoveData.MinSpeed < target.MoveData.Speed) ? target.MoveData.Speed - actor.MoveData.MinSpeed : 0;

        if (dist > FollowDistance)
          AdjustSpeed(actor, target.MoveData.Speed + (dist - FollowDistance) / SpeedAdjustmentDistanceRange * addspd);
        else
          AdjustSpeed(actor, target.MoveData.Speed - (FollowDistance - dist) / SpeedAdjustmentDistanceRange * subspd);

        Complete |= (!target.Active);
      }

      TV_3DVECTOR vNormal = new TV_3DVECTOR();
      TV_3DVECTOR vImpact = new TV_3DVECTOR();
      if (CheckImminentCollision(actor))
      {
        CollisionSystem.CreateAvoidAction(engine, actor);
      }
    }
  }
}
