using Primrose.Primitives.Factories;
using SWEndor.Actors;
using SWEndor.Core;

namespace SWEndor.AI.Actions
{
  internal class EnableSpawn : ActionInfo
  {

    private static ObjectPool<EnableSpawn> _pool;
    static EnableSpawn() { _pool = ObjectPool<EnableSpawn>.CreateStaticPool(() => { return new EnableSpawn(); }, (a) => { a.Reset(); }); }

    private EnableSpawn() : base("EnableSpawn") { CanInterrupt = false; }

    public static EnableSpawn GetOrCreate(bool enabled)
    {
      EnableSpawn h = _pool.GetNew();

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

    }

    private bool Enabled;

    public override void Process(Engine engine, ActorInfo actor)
    {
      actor.SpawnerInfo.Enabled = Enabled;
      Complete = true;
    }
  }
}
