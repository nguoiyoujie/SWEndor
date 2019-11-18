using Primrose.Primitives.Factories;
using SWEndor.Actors;
using SWEndor.Core;

namespace SWEndor.AI.Actions
{
  internal class Wait : ActionInfo
  {
    internal static int _count = 0;
    internal static ObjectPool<Wait> _pool = new ObjectPool<Wait>(() => { return new Wait(); }, (a) => { a.Reset(); });

    private Wait() : base("Wait") { }

    public static Wait GetOrCreate(float time = 5)
    {
      Wait h = _pool.GetNew();
      _count++;
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
      _count--;
    }

    private float WaitTime = 0;
    private float ResumeTime = 0;

    public override string ToString()
    {
      return string.Join(",", new string[]
      {
          Name
        , (ResumeTime - Globals.Engine.Game.GameTime).ToString()
        , Complete.ToString()
      });
    }

    public override void Process(Engine engine, ActorInfo actor)
    {
      if (ResumeTime == 0)
        ResumeTime = engine.Game.GameTime + WaitTime;

      actor.AI.SetTarget(actor.GetRelativePositionXYZ(0, 0, 1000));
      actor.AI.AdjustRotation(engine, actor);

      if (CheckImminentCollision(actor))
        CreateAvoidAction(actor);

      Complete |= (ResumeTime < engine.Game.GameTime);
    }
  }
}
