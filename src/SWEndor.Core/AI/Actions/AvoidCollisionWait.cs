﻿using SWEndor.Actors;
using SWEndor.Core;
using Primrose.Primitives.Factories;

namespace SWEndor.AI.Actions
{
  internal class AvoidCollisionWait : ActionInfo
  {

    private static ObjectPool<AvoidCollisionWait> _pool;
    static AvoidCollisionWait() { _pool = ObjectPool<AvoidCollisionWait>.CreateStaticPool(() => { return new AvoidCollisionWait(); }, (a) => { a.Reset(); }); }

    private AvoidCollisionWait() : base("AvoidCollisionWait") { }

    private float WaitTime = 0;
    private float ResumeTime = 0;

    public static AvoidCollisionWait GetOrCreate(float time = 5)
    {
      AvoidCollisionWait h = _pool.GetNew();

      h.WaitTime = time;
      h.CanInterrupt = false;
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
      actor.AI.AdjustRotation(engine, actor, 0.5f);

      actor.AI.SetTargetSpeed(actor.MoveData.MaxSpeed);
      actor.AI.AdjustSpeed(actor, false);

      if (CheckImminentCollision(actor))
      {
        CreateAvoidAction(actor);
        Complete = true;
      }

      Complete |= (ResumeTime < engine.Game.GameTime);
    }
  }
}
