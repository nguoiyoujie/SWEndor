using MTV3D65;
using System;
using System.Collections.Generic;
using System.Text;

namespace SWEndor.Actions
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
      return string.Format("{0},{1},{2},{3},{4},{5}"
                          , Name
                          , (Target_Actor != null) ? Target_Actor.ID : -1
                          , FollowDistance
                          , SpeedAdjustmentDistanceRange
                          , CanInterrupt
                          , Complete
                          );
    }

    public override void Process(ActorInfo owner)
    {
      if (owner.MaxSpeed == 0)
      {
        Complete = true;
        return;
      }

      if (CheckBounds(owner))
      {
        AdjustRotation(owner, Target_Actor.GetPosition());
        float dist = ActorDistanceInfo.GetDistance(owner, Target_Actor, FollowDistance + 1);

        float addspd = (owner.MaxSpeed > Target_Actor.Speed) ? owner.MaxSpeed - Target_Actor.Speed : 0;
        float subspd = (owner.MinSpeed < Target_Actor.Speed) ? Target_Actor.Speed - owner.MinSpeed : 0;

        if (dist > FollowDistance)
          AdjustSpeed(owner, Target_Actor.Speed + (dist - FollowDistance) / SpeedAdjustmentDistanceRange * addspd);
        else
          AdjustSpeed(owner, Target_Actor.Speed - (FollowDistance - dist) / SpeedAdjustmentDistanceRange * subspd);

        Complete |= (Target_Actor.CreationState != CreationState.ACTIVE);
      }

      TV_3DVECTOR vNormal = new TV_3DVECTOR();
      TV_3DVECTOR vImpact = new TV_3DVECTOR();
      if (CheckImminentCollision(owner, FollowDistance, out vImpact, out vNormal))
      {
        ActionManager.QueueFirst(owner, new AvoidCollisionRotate(vImpact, vNormal));
      }
    }
  }
}
