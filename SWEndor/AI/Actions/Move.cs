using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Primitives;

namespace SWEndor.AI.Actions
{
  public class Move : ActionInfo
  {
    public Move(TV_3DVECTOR target_position, float speed, float close_enough_distance = -1, bool can_interrupt = true) : base("Move")
    {
      Target_Position = target_position;
      Target_Speed = speed;
      CloseEnoughDistance = close_enough_distance;
      CanInterrupt = can_interrupt;
    }

    // parameters
    public TV_3DVECTOR Target_Position = new TV_3DVECTOR();
    public float Target_Speed = 0;
    public float CloseEnoughDistance = -1;

    public override string ToString()
    {
      return string.Join(",", new string[]
      {
          Name
        , Utilities.ToString(Target_Position)
        , Target_Speed.ToString()
        , CloseEnoughDistance.ToString()
        , CanInterrupt.ToString()
        , Complete.ToString()
      });
    }

    public override void Process(Engine engine, ActorInfo actor)
    {
      if (actor.MoveData.MaxSpeed == 0)
      {
        Complete = true;
        return;
      }

      if (CheckBounds(actor))
      {
        if (CloseEnoughDistance < 0)
          CloseEnoughDistance = actor.TypeInfo.AIData.Move_CloseEnough;

        actor.AIData.SetTarget(Target_Position);
        actor.AIData.AdjustRotation(actor);

        actor.AIData.SetTargetSpeed(Target_Speed);
        actor.AIData.AdjustSpeed(actor);

        float dist = engine.TrueVision.TVMathLibrary.GetDistanceVec3D(actor.GetGlobalPosition(), Target_Position);
        Complete |= (dist <= CloseEnoughDistance);
      }

      TV_3DVECTOR vNormal = new TV_3DVECTOR();
      TV_3DVECTOR vImpact = new TV_3DVECTOR();
      if (CheckImminentCollision(actor))
      {
        CreateAvoidAction(actor);
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
