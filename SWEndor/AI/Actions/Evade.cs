using MTV3D65;
using System;
using System.Collections.Generic;
using System.Text;

namespace SWEndor.Actions
{
  public class Evade : ActionInfo
  {
    public Evade(float time = 2.5f) : base("Evade")
    {
      WaitTime = time;
      CanInterrupt = false;
    }

    // parameters
    public TV_3DVECTOR Target_Position = new TV_3DVECTOR();
    public float Target_Speed = 0;
    public float CloseEnoughAngle = 0.1f;
    private float WaitTime = 0;
    private float ResumeTime = 0;
    private bool poschecked = false;


    public override string ToString()
    {
      return string.Format("{0},{1},{2}"
                          , Name
                          , ResumeTime - Game.Instance().GameTime
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

      if (ResumeTime == 0)
      {
        ResumeTime = Game.Instance().GameTime + WaitTime;
      }

      if (CheckBounds(owner))
      {
        if (!poschecked)
        {
          poschecked = true;
          if (owner.IsNearlyOutOfBounds())
            Target_Position = new TV_3DVECTOR();
          else
            Target_Position = new TV_3DVECTOR(100, Engine.Instance().Random.Next(-50, 50), Engine.Instance().Random.Next(-50, 50));
        }

        float delta_angle = AdjustRotation(owner, owner.GetRelativePositionFUR(Target_Position.x, Target_Position.y, Target_Position.z));
        float delta_speed = AdjustSpeed(owner, owner.Speed);

        Complete |= (delta_angle <= CloseEnoughAngle && delta_angle >= -CloseEnoughAngle && delta_speed == 0);
        Complete |= (ResumeTime < Game.Instance().GameTime);
      }

      TV_3DVECTOR vNormal = new TV_3DVECTOR();
      TV_3DVECTOR vImpact = new TV_3DVECTOR();
      if (CheckImminentCollision(owner, owner.Speed * 3, out vImpact, out vNormal))
      {
        ActionManager.QueueFirst(owner, new AvoidCollisionRotate(vImpact, vNormal));
      }
    }
  }
}
