using MTV3D65;
using System;
using System.Collections.Generic;
using System.Text;

namespace SWEndor.Actions
{
  public class AvoidCollisionRotate : ActionInfo
  {
    public AvoidCollisionRotate(TV_3DVECTOR impact_position, TV_3DVECTOR normal_vec, float close_enough_angle = 0.1f) : base("AvoidCollisionRotate")
    {
      Impact_Position = impact_position;
      Normal = normal_vec;
      CloseEnoughAngle = close_enough_angle;
      CanInterrupt = false;
    }

    // parameters
    public TV_3DVECTOR Impact_Position = new TV_3DVECTOR();
    public TV_3DVECTOR Target_Position = new TV_3DVECTOR();
    public TV_3DVECTOR Normal = new TV_3DVECTOR();
    public float CloseEnoughAngle = 0.1f;

    public override string ToString()
    {
      return string.Format("{0},{1},{2},{3},{4}"
                          , Name
                          , Utilities.ToString(Impact_Position)
                          , Utilities.ToString(Normal)
                          , CloseEnoughAngle
                          , Complete
                          );
    }

    public override void Process(ActorInfo owner)
    {
      if (owner.MaxTurnRate == 0)
      {
        Complete = true;
        return;
      }

      if (CheckBounds(owner))
      {
        Target_Position = Impact_Position + Normal * 1000;
        //Target_Position = owner.ProspectiveCollisionGoodLocation; //Impact_Position + Normal * 1000;
        float dist = Engine.Instance().TVMathLibrary.GetDistanceVec3D(owner.GetPosition(), Target_Position);
        float Target_Speed = dist / 10;

        float delta_angle = AdjustRotation(owner, Target_Position);
        float delta_speed = AdjustSpeed(owner, Target_Speed);

        Complete |= (delta_angle <= CloseEnoughAngle && delta_angle >= -CloseEnoughAngle && delta_speed == 0);
      }

      TV_3DVECTOR vNormal = new TV_3DVECTOR();
      TV_3DVECTOR vImpact = new TV_3DVECTOR();
      if (CheckImminentCollision(owner, owner.Speed * 10, out vImpact , out vNormal))
      {
        Impact_Position = vImpact;
        Normal = vNormal;
        Complete = false;
      }
      else
      {
        Complete = true;
      }
    }
  }
}
