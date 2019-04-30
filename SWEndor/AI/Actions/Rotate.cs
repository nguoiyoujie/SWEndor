using MTV3D65;
using SWEndor.Actors;

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
      return string.Format("{0},{1},{2},{3},{4},{5}"
                          , Name
                          , Utilities.ToString(Target_Position)
                          , Target_Speed
                          , CloseEnoughAngle
                          , CanInterrupt
                          , Complete
                          );
    }

    public override void Process(Engine engine, int actorID)
    {
      ActorInfo actor = engine.ActorFactory.Get(actorID);
      if (actor.MovementInfo.MaxTurnRate == 0)
      {
        Complete = true;
        return;
      }

      if (CheckBounds(actor))
      {
        float delta_angle = AdjustRotation(actor, Target_Position);
        float delta_speed = AdjustSpeed(actor, Target_Speed);

        Complete |= (delta_angle <= CloseEnoughAngle && delta_angle >= -CloseEnoughAngle && delta_speed == 0);
      }

      TV_3DVECTOR vNormal = new TV_3DVECTOR();
      TV_3DVECTOR vImpact = new TV_3DVECTOR();
      if (CheckImminentCollision(actor, actor.MovementInfo.Speed * 2.5f))
      {
        engine.ActionManager.QueueFirst(actorID, new AvoidCollisionRotate(actor.CollisionInfo.ProspectiveCollisionImpact, actor.CollisionInfo.ProspectiveCollisionNormal));
      }
    }
  }
}
