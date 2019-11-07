using SWEndor.Actors;
using SWEndor.Core;
using Primrose.Primitives.Factories;

namespace SWEndor.AI.Actions
{
  public class AvoidCollisionWait : ActionInfo
  {
    internal static int _count = 0;
    internal static ObjectPool<AvoidCollisionWait> _pool = new ObjectPool<AvoidCollisionWait>(() => { return new AvoidCollisionWait(); }, (a) => { a.Reset(); });

    private AvoidCollisionWait() : base("AvoidCollisionWait") { }

    private float WaitTime = 0;
    private float ResumeTime = 0;

    public static AvoidCollisionWait GetOrCreate(float time = 5)
    {
      AvoidCollisionWait h = _pool.GetNew();
      _count++;
      h.WaitTime = time;
      h.CanInterrupt = false;
      return h;
    }

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

      actor.AIData.SetTarget(actor.GetRelativePositionXYZ(0, 0, 1000));
      actor.AIData.AdjustRotation(engine, actor, 0.5f);

      actor.AIData.SetTargetSpeed(actor.MoveData.MaxSpeed);
      actor.AIData.AdjustSpeed(actor);

      if (CheckImminentCollision(actor))
      {
        CreateAvoidAction(actor);
        Complete = true;
      }

      Complete |= (ResumeTime < engine.Game.GameTime);
    }

    public override void Reset()
    {
      base.Reset();
      WaitTime = 0;
      ResumeTime = 0;
    }

    public override void Return()
    {
      base.Return();
      _pool.Return(this);
      _count--;
    }
  }
}
