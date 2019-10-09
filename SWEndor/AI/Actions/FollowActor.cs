using MTV3D65;
using SWEndor.Actors;
using SWEndor.Core;

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
      return string.Join(",", new string[]
      {
          Name
        , Target_ActorID.ToString()
        , FollowDistance.ToString()
        , SpeedAdjustmentDistanceRange.ToString()
        , CanInterrupt.ToString()
        , Complete.ToString()
      });
    }

    public override void Process(Engine engine, ActorInfo actor)
    {
      ActorInfo target = engine.ActorFactory.Get(Target_ActorID);
      if (target == null || actor.MoveData.MaxSpeed == 0)
      {
        Complete = true;
        return;
      }

      actor.AIData.SetTarget(engine, actor, target, false);

      if (CheckBounds(actor))
      {
        actor.AIData.AdjustRotation(engine, actor);
        float dist = ActorDistanceInfo.GetDistance(engine, actor, target, FollowDistance + 1);

        actor.AIData.AdjustSpeed(actor);
        Complete |= (!target.Active);
      }

      TV_3DVECTOR vNormal = new TV_3DVECTOR();
      TV_3DVECTOR vImpact = new TV_3DVECTOR();
      if (CheckImminentCollision(actor))
      {
        CreateAvoidAction(actor);
      }
      else
      {
        foreach (ActorInfo l in actor.Squad.Members)
        {
          if (l != null && actor != l && ActorDistanceInfo.GetRoughDistance(actor, l) < l.MoveData.Speed * 0.5f)
          {
            actor.QueueFirst(Evade.GetOrCreate(0.5f));
            break;
          }
          else if (actor == l)
            break;
        }
        //ActorInfo leader = actor.Squad.Leader;
        //if (leader != null && actor != leader && ActorDistanceInfo.GetRoughDistance(actor, leader) < leader.MoveData.Speed * 0.5f)
        //actor.QueueFirst(Evade.GetOrCreate(0.5f));
      }
    }
  }
}
