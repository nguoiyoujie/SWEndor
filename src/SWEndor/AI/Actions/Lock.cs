using Primrose.Primitives.Factories;
using SWEndor.Actors;
using SWEndor.Core;

namespace SWEndor.AI.Actions
{
  internal class Lock : ActionInfo
  {
    internal static int _count = 0;
    private static ObjectPool<Lock> _pool;
    static Lock() { _pool = ObjectPool<Lock>.CreateStaticPool(() => { return new Lock(); }, (a) => { a.Reset(); }); }

    private Lock() : base("Lock") { CanInterrupt = false; }

    public static Lock GetOrCreate()
    {
      Lock h = _pool.GetNew();
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
      actor.MoveData.ResetTurn();
    }
  }
}
