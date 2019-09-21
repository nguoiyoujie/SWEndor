using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Primitives;

namespace SWEndor.AI.Actions
{
  public class Rotate : ActionInfo
  {
    public Rotate(TV_3DVECTOR target_position, float speed, float close_enough_angle = 0.1f, bool can_interrupt = true) : base("Rotate")
    {
      Target_Position = target_position;
      Target_Speed = speed;
      CloseEnoughAngle = close_enough_angle;
      CanInterrupt = can_interrupt;
    }

    // parameters
    public TV_3DVECTOR Target_Position = new TV_3DVECTOR();
    public float Target_Speed = 0;
    public float CloseEnoughAngle = 0.1f;

    public override string ToString()
    {
      return string.Join(",", new string[]
      {
          Name
        , Utilities.ToString(Target_Position)
        , Target_Speed.ToString()
        , CloseEnoughAngle.ToString()
        , CanInterrupt.ToString()
        , Complete.ToString()
      });
    }

    public override void Process(Engine engine, ActorInfo actor)
    {
      if (actor.MoveData.MaxTurnRate == 0)
      {
        Complete = true;
        return;
      }

      if (CheckBounds(actor))
      {
        actor.AIData.SetTarget(Target_Position);
        float delta_angle = actor.AIData.AdjustRotation(actor);

        actor.AIData.SetTargetSpeed(Target_Speed);
        float delta_speed = actor.AIData.AdjustSpeed(actor);

        Complete |= (delta_angle <= CloseEnoughAngle && delta_angle >= -CloseEnoughAngle && delta_speed == 0);
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
