using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Actors.Data;

namespace SWEndor.AI.Actions
{
  public class Idle : ActionInfo
  {
    public Idle() : base("Idle")
    {
    }

    public override void Process(Engine engine, ActorInfo actor)
    {
      if (CheckBounds(actor))
      {
        AdjustRotation(actor, actor.GetPosition());
        AdjustSpeed(actor, actor.MoveData.MinSpeed);

        if (NextAction == null)
        {
          engine.ActionManager.QueueLast(actor.ID, new Hunt());
        }

        Complete = true;

        TV_3DVECTOR vNormal = new TV_3DVECTOR();
        TV_3DVECTOR vImpact = new TV_3DVECTOR();
        if (CheckImminentCollision(actor, actor.MoveData.Speed * 2.5f))
        {
          CollisionSystem.CreateAvoidAction(engine, actor);
        }
      }
    }
  }
}
