using Primrose.Primitives.Factories;
using SWEndor.Actors;
using SWEndor.Core;

namespace SWEndor.AI.Actions
{
  internal class Delete : ActionInfo
  {

    private static ObjectPool<Delete> _pool;
    static Delete() { _pool = ObjectPool<Delete>.CreateStaticPool(() => { return new Delete(); }, (a) => { a.Reset(); }); }

    private Delete() : base("Delete") { CanInterrupt = false; }

    public static Delete GetOrCreate()
    {
      Delete h = _pool.GetNew();

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
      actor.Delete();
      Complete = true;
    }
  }
}
