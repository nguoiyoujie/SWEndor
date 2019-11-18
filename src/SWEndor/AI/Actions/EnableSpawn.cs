using Primrose.Primitives.Factories;
using SWEndor.Actors;
using SWEndor.Core;

namespace SWEndor.AI.Actions
{
  internal class EnableSpawn : ActionInfo
  {
    internal static int _count = 0;
    internal static ObjectPool<EnableSpawn> _pool = new ObjectPool<EnableSpawn>(() => { return new EnableSpawn(); }, (a) => { a.Reset(); });

    private EnableSpawn() : base("EnableSpawn") { CanInterrupt = false; }

    public static EnableSpawn GetOrCreate(bool enabled)
    {
      EnableSpawn h = _pool.GetNew();
      _count++;
      h.Enabled = enabled;
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

    private bool Enabled;

    public override void Process(Engine engine, ActorInfo actor)
    {
      actor.SpawnerInfo.Enabled = Enabled;
      Complete = true;
    }
  }
}
