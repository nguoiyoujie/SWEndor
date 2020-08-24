using Primrose.Primitives.Factories;
using SWEndor.Game.Actors;
using SWEndor.Game.Core;

namespace SWEndor.Game.AI.Actions
{
  internal class Lock : ActionInfo
  {

    private static ObjectPool<Lock> _pool;
    static Lock() { _pool = ObjectPool<Lock>.CreateStaticPool(() => { return new Lock(); }, (a) => { a.Reset(); }); }

    private Lock() : base("Lock") { CanInterrupt = false; }

    public static Lock GetOrCreate()
    {
      Lock h = _pool.GetNew();

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
      actor.MoveData.ResetTurn();
    }
  }
}
