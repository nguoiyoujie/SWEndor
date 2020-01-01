using Primrose.Primitives.Factories;
using SWEndor.Actors;
using SWEndor.Core;

namespace SWEndor.AI.Actions
{
  internal class SetGameStateB : ActionInfo
  {

    private static ObjectPool<SetGameStateB> _pool;
    static SetGameStateB() { _pool = ObjectPool<SetGameStateB>.CreateStaticPool(() => { return new SetGameStateB(); }, (a) => { a.Reset(); }); }

    private SetGameStateB() : base("SetGameStateB") { CanInterrupt = false; }

    public static SetGameStateB GetOrCreate(string key, bool state)
    {
      SetGameStateB h = _pool.GetNew();

      h.m_key = key;
      h.m_state = state;
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

    private string m_key;
    private bool m_state;

    public override void Process(Engine engine, ActorInfo actor)
    {
      engine.GameScenarioManager.Scenario.State.SetGameStateB(m_key, m_state);
      Complete = true;
    }
  }
}
