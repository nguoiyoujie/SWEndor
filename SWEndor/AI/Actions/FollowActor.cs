using MTV3D65;
using SWEndor.Actors;

namespace SWEndor.AI.Actions
{
  public class FollowActor : ActionInfo
  {
    public FollowActor(int targetActorID, float follow_distance = 500, bool can_interrupt = true) : base("FollowActor")
    {
      Target_ActorID = targetActorID;
      FollowDistance = follow_distance;
      CanInterrupt = can_interrupt;
    }

    // parameters
    public int Target_ActorID = -1;
    public float FollowDistance = 500;
    public float SpeedAdjustmentDistanceRange = 100;

    public override string ToString()
    {
      return string.Format("{0},{1},{2},{3},{4},{5}"
                          , Name
                          , Target_ActorID
                          , FollowDistance
                          , SpeedAdjustmentDistanceRange
                          , CanInterrupt
                          , Complete
                          );
    }

    public override void Process(Engine engine, int actorID)
    {
      ActorInfo actor = engine.ActorFactory.Get(actorID);
      ActorInfo target = engine.ActorFactory.Get(Target_ActorID);
      if (target == null || actor.MoveComponent.MaxSpeed == 0)
      {
        Complete = true;
        return;
      }

      if (CheckBounds(actor))
      {
        AdjustRotation(actor, target.GetPosition());
        float dist = ActorDistanceInfo.GetDistance(actorID, Target_ActorID, FollowDistance + 1);

        float addspd = (actor.MoveComponent.MaxSpeed > target.MoveComponent.Speed) ? actor.MoveComponent.MaxSpeed - target.MoveComponent.Speed : 0;
        float subspd = (actor.MoveComponent.MinSpeed < target.MoveComponent.Speed) ? target.MoveComponent.Speed - actor.MoveComponent.MinSpeed : 0;

        if (dist > FollowDistance)
          AdjustSpeed(actor, target.MoveComponent.Speed + (dist - FollowDistance) / SpeedAdjustmentDistanceRange * addspd);
        else
          AdjustSpeed(actor, target.MoveComponent.Speed - (FollowDistance - dist) / SpeedAdjustmentDistanceRange * subspd);

        Complete |= (target.CreationState != CreationState.ACTIVE);
      }

      TV_3DVECTOR vNormal = new TV_3DVECTOR();
      TV_3DVECTOR vImpact = new TV_3DVECTOR();
      if (CheckImminentCollision(actor, actor.MoveComponent.Speed * 2.5f))
      {
        engine.ActionManager.QueueFirst(actorID, new AvoidCollisionRotate(actor.CollisionInfo.ProspectiveCollisionImpact, actor.CollisionInfo.ProspectiveCollisionNormal));
      }
    }
  }
}
