using MTV3D65;
using System;
using System.Collections.Generic;
using System.Text;

namespace SWEndor.Actions
{
  public class Lock : ActionInfo
  {
    public Lock() : base("Lock")
    {
      CanInterrupt = false;
    }

    public override void Process(ActorInfo owner)
    {
      owner.XTurnAngle = 0;
      owner.YTurnAngle = 0;

    }
  }
}
