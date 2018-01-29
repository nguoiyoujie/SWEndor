using MTV3D65;
using System;
using System.Collections.Generic;
using System.Text;

namespace SWEndor.Actions
{
  public class HyperspaceIn : ActionInfo 
  {
    public HyperspaceIn(TV_3DVECTOR target_position) : base("HyperspaceIn")
    {
      Target_Position = target_position;
      CanInterrupt = false;
    }

    // parameters
    private TV_3DVECTOR Target_Position = new TV_3DVECTOR();
    private float Max_Speed = 25000;
    private float Min_Speed = 1000;
    private float SpeedDistanceFactor = 2.5f;
    private float CloseEnoughDistance = 500;
    private ActorState prevState = ActorState.NORMAL;
    private float prevdist = 9999999;
    private bool m_switch = false;

    public override string ToString()
    {
      return string.Format("{0},{1},{2}"
                          , Name
                          , Utilities.ToString(Target_Position)
                          , Complete
                          );
    }

    public override void Process(ActorInfo owner)
    {
      if (owner.ActorState != ActorState.HYPERSPACE && !m_switch)
      {
        prevState = owner.ActorState;
        owner.ActorState = ActorState.HYPERSPACE;
        owner.LookAtPoint(Target_Position);
        m_switch = true;
      }

      //Complete = (owner.ActorState != ActorState.HYPERSPACE);
    }

    public void ApplyMove(ActorInfo owner)
    {
      float dist = Engine.Instance().TVMathLibrary.GetDistanceVec3D(owner.GetPosition(), Target_Position);

      if (dist <= CloseEnoughDistance || prevdist < dist)
      {
        owner.ActorState = prevState;
        owner.Speed = owner.MaxSpeed;
        //owner.SetLocalPosition(Target_Position.x, Target_Position.y, Target_Position.z);
        Complete = true;
      }
      else
      {
        if (owner.ActorState != ActorState.HYPERSPACE)
        {
          prevState = owner.ActorState;
          owner.ActorState = ActorState.HYPERSPACE;
          owner.LookAtPoint(Target_Position);
        }

        owner.Speed = Min_Speed + dist * SpeedDistanceFactor;
        if (owner.Speed > Max_Speed)
          owner.Speed = Max_Speed;

        //AdjustSpeed(owner, owner.Speed);

        Complete = false;
      }

      prevdist = dist;
    }
  }
}
