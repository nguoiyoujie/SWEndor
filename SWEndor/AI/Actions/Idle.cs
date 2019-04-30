using MTV3D65;
using SWEndor.Actors;

namespace SWEndor.AI.Actions
{
  public class Idle : ActionInfo
  {
    public Idle() : base("Idle")
    {
    }

    public override void Process(Engine engine, int actorID)
    {
      ActorInfo actor = engine.ActorFactory.Get(actorID);
      if (CheckBounds(actor))
      {
        AdjustRotation(actor, actor.GetPosition());
        AdjustSpeed(actor, actor.MoveComponent.MinSpeed);

        if (NextAction == null)
        {
          engine.ActionManager.QueueLast(actorID, new Hunt());
        }

        Complete = true;

        TV_3DVECTOR vNormal = new TV_3DVECTOR();
        TV_3DVECTOR vImpact = new TV_3DVECTOR();
        if (CheckImminentCollision(actor, actor.MoveComponent.Speed * 2.5f))
        {
          engine.ActionManager.QueueFirst(actorID, new AvoidCollisionRotate(actor.CollisionInfo.ProspectiveCollisionImpact, actor.CollisionInfo.ProspectiveCollisionNormal));
        }
      }
    }
  }
}
