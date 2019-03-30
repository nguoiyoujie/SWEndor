using MTV3D65;
using SWEndor.Actors;

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
      return string.Format("{0},{1},{2},{3},{4},{5}"
                          , Name
                          , Utilities.ToString(Target_Position)
                          , Target_Speed
                          , CloseEnoughDistance
                          , CanInterrupt
                          , Complete
                          );
    }

    public override void Process(ActorInfo owner)
    {
      if (owner.MovementInfo.MaxSpeed == 0)
      {
        Complete = true;
        return;
      }

      if (CheckBounds(owner))
      {
        if (CloseEnoughDistance < 0)
          CloseEnoughDistance = owner.TypeInfo.Move_CloseEnough;

        AdjustRotation(owner, Target_Position);
        AdjustSpeed(owner, Target_Speed);

        float dist = Engine.Instance().TVMathLibrary.GetDistanceVec3D(owner.GetPosition(), Target_Position);
        Complete |= (dist <= CloseEnoughDistance);
      }

      TV_3DVECTOR vNormal = new TV_3DVECTOR();
      TV_3DVECTOR vImpact = new TV_3DVECTOR();
      if (CheckImminentCollision(owner, owner.MovementInfo.Speed * 2.5f))
      {
        ActionManager.QueueFirst(owner, new AvoidCollisionRotate(owner.ProspectiveCollisionImpact, owner.ProspectiveCollisionNormal));
      }
    }
  }
}
