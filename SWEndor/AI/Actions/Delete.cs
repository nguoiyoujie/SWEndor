using MTV3D65;
using System;
using System.Collections.Generic;
using System.Text;

namespace SWEndor.Actions
{
  public class Delete : ActionInfo
  {
    public Delete() : base("Delete")
    {
      CanInterrupt = false;
    }

    public override void Process(ActorInfo owner)
    {
      owner.Destroy();
      Complete = true;
    }
  }
}
