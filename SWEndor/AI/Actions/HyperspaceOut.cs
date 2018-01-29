using MTV3D65;
using System;
using System.Collections.Generic;
using System.Text;

namespace SWEndor.Actions
{
  public class HyperspaceOut : ActionInfo
  {
    public HyperspaceOut() : base("HyperspaceOut")
    {
      CanInterrupt = false;
    }

    // parameters
    private TV_3DVECTOR Origin_Position = new TV_3DVECTOR();
    private float Target_Speed = 1000;
    private float FarEnoughDistance = 50000;
    private ActorState prevState = ActorState.NORMAL;
    private bool m_switch = false;

    public override void Process(ActorInfo owner)
    {
      if (owner.ActorState != ActorState.HYPERSPACE && !m_switch)
      {
        prevState = owner.ActorState;
        owner.ActorState = ActorState.HYPERSPACE;
        Origin_Position = owner.GetPosition();
      }
    }

    public void ApplyMove(ActorInfo owner)
    {
      //AdjustSpeed(owner, Target_Speed);
      owner.Speed += Target_Speed;

      float dist = Engine.Instance().TVMathLibrary.GetDistanceVec3D(owner.GetPosition(), Origin_Position);
      if (dist >= FarEnoughDistance)
      {
        owner.ActorState = prevState;
        Complete = true;
      }
      else
      {
        Complete = false;
      }
    }
  }
}
