using Primrose.Primitives.Factories;
using SWEndor.Game.Actors;
using SWEndor.Game.Core;
using System;

namespace SWEndor.Game.AI.Actions
{
  internal class CustomAction : ActionInfo
  {

    private static ObjectPool<CustomAction> _pool;
    static CustomAction() { _pool = ObjectPool<CustomAction>.CreateStaticPool(() => { return new CustomAction(); }, (a) => { a.Reset(); }); }

    private CustomAction() : base("CustomAction") { CanInterrupt = false; }

    public static CustomAction GetOrCreate(Action action)
    {
      CustomAction h = _pool.GetNew();

      h.Action = action;
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

    private Action Action;

    public override void Process(Engine engine, ActorInfo actor)
    {
      Action.Invoke();
      Complete = true;
    }
  }
}
