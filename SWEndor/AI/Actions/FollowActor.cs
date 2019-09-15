using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Primitives;

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
      return "{0},{1},{2},{3},{4},{5}".F(Name
                          , Target_ActorID
                          , FollowDistance
                          , SpeedAdjustmentDistanceRange
                          , CanInterrupt
                          , Complete
                          );
    }

    public override void Process(Engine engine, ActorInfo actor)
    {
      ActorInfo target = engine.ActorFactory.Get(Target_ActorID);
      if (target == null || actor.MoveData.MaxSpeed == 0)
      {
        Complete = true;
        return;
      }

      actor.AIData.SetTarget(actor, target, false);

      if (CheckBounds(actor))
      {
        actor.AIData.AdjustRotation(actor);
        float dist = ActorDistanceInfo.GetDistance(actor, target, FollowDistance + 1);

        actor.AIData.AdjustSpeed(actor);
        Complete |= (!target.Active);
      }

      TV_3DVECTOR vNormal = new TV_3DVECTOR();
      TV_3DVECTOR vImpact = new TV_3DVECTOR();
      if (CheckImminentCollision(actor))
      {
        CollisionSystem.CreateAvoidAction(engine, actor);
      }
      else
      {
        ActorInfo leader = actor.Squad.Leader;
        if (leader != null && actor != leader && ActorDistanceInfo.GetRoughDistance(actor, leader) < leader.MoveData.Speed * 0.5f)
        {
          actor.QueueFirst(new Evade(0.5f));
        }
      }
    }
  }
}
