using MTV3D65;
using Primrose.Primitives.Factories;
using SWEndor.Actors;
using SWEndor.Core;

namespace SWEndor.AI.Actions
{
  internal class Idle : ActionInfo
  {
    internal static int _count = 0;
    internal static ObjectPool<Idle> _pool = new ObjectPool<Idle>(() => { return new Idle(); }, (a) => { a.Reset(); });

    private Idle() : base("Idle") { }

    public static Idle GetOrCreate()
    {
      Idle h = _pool.GetNew();
      _count++;
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
      _count--;
    }

    public override void Process(Engine engine, ActorInfo actor)
    {
      if (CheckBounds(actor))
      {
        //AdjustRotation(engine, actor, actor.GetGlobalPosition());
        actor.AI.SetTargetSpeed(actor.MoveData.MinSpeed);
        actor.AI.AdjustSpeed(actor);

        if (NextAction == null)
          actor.QueueLast(Hunt.GetOrCreate());

        Complete = true;

        TV_3DVECTOR vNormal = new TV_3DVECTOR();
        TV_3DVECTOR vImpact = new TV_3DVECTOR();
        if (CheckImminentCollision(actor))
        {
          CreateAvoidAction(actor);
        }
      }
    }
  }
}
