using Primrose.Primitives.Factories;
using SWEndor.Actors;
using SWEndor.Core;

namespace SWEndor.AI.Actions
{
  internal class SelfDestruct : ActionInfo
  {
    internal static int _count = 0;
    internal static ObjectPool<SelfDestruct> _pool = new ObjectPool<SelfDestruct>(() => { return new SelfDestruct(); }, (a) => { a.Reset(); });

    private SelfDestruct() : base("SelfDestruct") { CanInterrupt = false; }
   
    public static SelfDestruct GetOrCreate()
    {
      SelfDestruct h = _pool.GetNew();
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
      actor.SetState_Dead();
      Complete = true;
    }
  }
}
