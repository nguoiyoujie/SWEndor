using MTV3D65;
using Primrose.Primitives.Factories;
using SWEndor.Game.Actors;
using SWEndor.Game.Core;

namespace SWEndor.Game.AI.Actions
{
  internal class Idle : ActionInfo
  {

    private static ObjectPool<Idle> _pool;
    static Idle() { _pool = ObjectPool<Idle>.CreateStaticPool(() => { return new Idle(); }, (a) => { a.Reset(); }); }

    private Idle() : base("Idle") { }

    public static Idle GetOrCreate()
    {
      Idle h = _pool.GetNew();

      h.IsDisposed = false;
      return h;
    }

    public override void Reset()
    {
      base.Reset();
    }

    public override void Return()
    {
      base.Return();
      _pool.Return(this);

    }

    public override void Process(Engine engine, ActorInfo actor)
    {
      if (CheckBounds(actor))
      {
        actor.AI.SetTargetSpeed(actor.MoveData.MinSpeed);
        actor.AI.AdjustSpeed(actor, false);

        Complete = true;

        TV_3DVECTOR vNormal = new TV_3DVECTOR();
        TV_3DVECTOR vImpact = new TV_3DVECTOR();
        if (CheckImminentCollision(actor))
        {
          CreateAvoidAction(actor);
        }
        else
        {
          if (NextAction == null)
            actor.AIDecision.OnIdleAction?.Invoke(actor);
        }
      }
    }
  }
}
