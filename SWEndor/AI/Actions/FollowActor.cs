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

    public override void Process(ActorInfo owner)
    {
      ActorInfo target = owner.Owner.Engine.ActorFactory.Get(Target_ActorID);
      if (target == null || owner.MovementInfo.MaxSpeed == 0)
      {
        Complete = true;
        return;
      }

      if (CheckBounds(owner))
      {
        AdjustRotation(owner, target.GetPosition());
        float dist = ActorDistanceInfo.GetDistance(owner.ID, Target_ActorID, FollowDistance + 1);

        float addspd = (owner.MovementInfo.MaxSpeed > target.MovementInfo.Speed) ? owner.MovementInfo.MaxSpeed - target.MovementInfo.Speed : 0;
        float subspd = (owner.MovementInfo.MinSpeed < target.MovementInfo.Speed) ? target.MovementInfo.Speed - owner.MovementInfo.MinSpeed : 0;

        if (dist > FollowDistance)
          AdjustSpeed(owner, target.MovementInfo.Speed + (dist - FollowDistance) / SpeedAdjustmentDistanceRange * addspd);
        else
          AdjustSpeed(owner, target.MovementInfo.Speed - (FollowDistance - dist) / SpeedAdjustmentDistanceRange * subspd);

        Complete |= (target.CreationState != CreationState.ACTIVE);
      }

      TV_3DVECTOR vNormal = new TV_3DVECTOR();
      TV_3DVECTOR vImpact = new TV_3DVECTOR();
      if (CheckImminentCollision(owner, owner.MovementInfo.Speed * 2.5f))
      {
        owner.Owner.Engine.ActionManager.QueueFirst(owner.ID, new AvoidCollisionRotate(owner.CollisionInfo.ProspectiveCollisionImpact, owner.CollisionInfo.ProspectiveCollisionNormal));
      }
    }
  }
}
