using MTV3D65;
using System;
using System.Collections.Generic;
using System.Text;

namespace SWEndor.Actions
{
  public class Idle : ActionInfo
  {
    public Idle() : base("Idle")
    {
    }

    public override void Process(ActorInfo owner)
    {
      if (CheckBounds(owner))
      {
        AdjustRotation(owner, owner.GetPosition());
        AdjustSpeed(owner, owner.MinSpeed);

        if (NextAction == null)
        {
          ActionManager.QueueLast(owner, new Hunt());
        }

        Complete = true;

        TV_3DVECTOR vNormal = new TV_3DVECTOR();
        TV_3DVECTOR vImpact = new TV_3DVECTOR();
        if (CheckImminentCollision(owner, owner.Speed * 3, out vImpact, out vNormal))
        {
          ActionManager.QueueFirst(owner, new AvoidCollisionRotate(vImpact, vNormal));
        }
      }
    }
  }
}
