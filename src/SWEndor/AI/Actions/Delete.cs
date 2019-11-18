using Primrose.Primitives.Factories;
using SWEndor.Actors;
using SWEndor.Core;

namespace SWEndor.AI.Actions
{
  internal class Delete : ActionInfo
  {
    internal static int _count = 0;
    internal static ObjectPool<Delete> _pool = new ObjectPool<Delete>(() => { return new Delete(); }, (a) => { a.Reset(); });

    private Delete() : base("Delete") { CanInterrupt = false; }

    public static Delete GetOrCreate()
    {
      Delete h = _pool.GetNew();
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
      actor.Delete();
      Complete = true;
    }
  }
}
