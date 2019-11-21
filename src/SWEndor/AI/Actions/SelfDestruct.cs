using Primrose.Primitives.Factories;
using SWEndor.Actors;
using SWEndor.Core;

namespace SWEndor.AI.Actions
{
  internal class SelfDestruct : ActionInfo
  {

    private static ObjectPool<SelfDestruct> _pool;
    static SelfDestruct() { _pool = ObjectPool<SelfDestruct>.CreateStaticPool(() => { return new SelfDestruct(); }, (a) => { a.Reset(); }); }

    private SelfDestruct() : base("SelfDestruct") { CanInterrupt = false; }
   
    public static SelfDestruct GetOrCreate()
    {
      SelfDestruct h = _pool.GetNew();

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
      actor.SetState_Dead();
      Complete = true;
    }
  }
}
