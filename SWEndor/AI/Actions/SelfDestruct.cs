using MTV3D65;
using System;
using System.Collections.Generic;
using System.Text;

namespace SWEndor.Actions
{
  public class SelfDestruct : ActionInfo
  {
    public SelfDestruct() : base("SelfDestruct")
    {
      CanInterrupt = false;
    }

    public override void Process(ActorInfo owner)
    {
      owner.ActorState = ActorState.DEAD;
      Complete = true;
    }
  }
}
