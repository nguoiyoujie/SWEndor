using MTV3D65;
using SWEndor.Actors;

namespace SWEndor.AI.Actions
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
        AdjustSpeed(owner, owner.MovementInfo.MinSpeed);

        if (NextAction == null)
        {
          ActionManager.QueueLast(owner.ID, new Hunt());
        }

        Complete = true;

        TV_3DVECTOR vNormal = new TV_3DVECTOR();
        TV_3DVECTOR vImpact = new TV_3DVECTOR();
        if (CheckImminentCollision(owner, owner.MovementInfo.Speed * 2.5f))
        {
          ActionManager.QueueFirst(owner.ID, new AvoidCollisionRotate(owner.CollisionInfo.ProspectiveCollisionImpact, owner.CollisionInfo.ProspectiveCollisionNormal));
        }
      }
    }
  }
}
