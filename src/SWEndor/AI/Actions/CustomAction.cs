using Primrose.Primitives.Factories;
using SWEndor.Actors;
using SWEndor.Core;
using System;

namespace SWEndor.AI.Actions
{
  internal class CustomAction : ActionInfo
  {
    internal static int _count = 0;
    private static ObjectPool<CustomAction> _pool;
    static CustomAction() { _pool = ObjectPool<CustomAction>.CreateStaticPool(() => { return new CustomAction(); }, (a) => { a.Reset(); }); }

    private CustomAction() : base("CustomAction") { CanInterrupt = false; }

    public static CustomAction GetOrCreate(Action action)
    {
      CustomAction h = _pool.GetNew();
      _count++;
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
      _count--;
    }

    private Action Action;

    public override void Process(Engine engine, ActorInfo actor)
    {
      Action.Invoke();
      Complete = true;
    }
  }
}
