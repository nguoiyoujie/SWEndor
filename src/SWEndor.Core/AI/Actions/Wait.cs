﻿using Primrose.Primitives.Factories;
using SWEndor.Actors;
using SWEndor.Core;

namespace SWEndor.AI.Actions
{
  internal class Wait : ActionInfo
  {

    private static ObjectPool<Wait> _pool;
    static Wait() { _pool = ObjectPool<Wait>.CreateStaticPool(() => { return new Wait(); }, (a) => { a.Reset(); }); }

    private Wait() : base("Wait") { }

    public static Wait GetOrCreate(float time = 5)
    {
      Wait h = _pool.GetNew();

      h.WaitTime = time;
      h.IsDisposed = false;
      return h;
    }

    public override void Reset()
    {
      base.Reset();
      ResumeTime = 0;
    }

    public override void Return()
    {
      base.Return();
      _pool.Return(this);

    }

    private float WaitTime = 0;
    private float ResumeTime = 0;

    public override string ToString()
    {
      return string.Join(",", new string[]
      {
          Name
        , ResumeTime.ToString()
        , Complete.ToString()
      });
    }

    public override void Process(Engine engine, ActorInfo actor)
    {
      if (ResumeTime == 0)
        ResumeTime = engine.Game.GameTime + WaitTime;

      actor.AI.Target.Set(actor.GetRelativePositionXYZ(0, 0, 1000));
      actor.AI.AdjustRotation(engine, actor);

      if (CheckImminentCollision(actor))
        CreateAvoidAction(actor);

      Complete |= (ResumeTime < engine.Game.GameTime);
    }
  }
}
