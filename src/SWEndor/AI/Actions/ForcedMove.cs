using MTV3D65;
using Primrose.Primitives.Factories;
using SWEndor.Actors;
using SWEndor.Core;
using SWEndor.Primitives.Extensions;

namespace SWEndor.AI.Actions
{
  internal class ForcedMove : ActionInfo
  {
    internal static int _count = 0;
    private static ObjectPool<ForcedMove> _pool;
    static ForcedMove() { _pool = ObjectPool<ForcedMove>.CreateStaticPool(() => { return new ForcedMove(); }, (a) => { a.Reset(); }); }

    private ForcedMove() : base("ForcedMove") { CanInterrupt = false; }

    public static ForcedMove GetOrCreate(TV_3DVECTOR target_position, float speed, float close_enough_distance = -1, float expire_time = 999999)
    {
      ForcedMove h = _pool.GetNew();
      _count++;
      h.Target_Position = target_position;
      h.Target_Speed = speed;
      h.CloseEnoughDistance = close_enough_distance;
      h.WaitTime = expire_time;
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

    // parameters
    public TV_3DVECTOR Target_Position = new TV_3DVECTOR();
    public float Target_Speed = 0;
    public float CloseEnoughDistance = -1;
    private float WaitTime = 0;
    private float ResumeTime = 0;

    public override string ToString()
    {
      return string.Join(",", new string[]
      {
          Name
        , Target_Position.Str()
        , Target_Speed.ToString()
        , CloseEnoughDistance.ToString()
        , ResumeTime.ToString()
        , CanInterrupt.ToString()
        , Complete.ToString()
      });
    }

    public override void Process(Engine engine, ActorInfo actor)
    {
      if (actor.MoveData.MaxSpeed == 0)
      {
        Complete = true;
        return;
      }

      if (ResumeTime == 0)
      {
        ResumeTime = engine.Game.GameTime + WaitTime;
      }

      if (CloseEnoughDistance < 0)
        CloseEnoughDistance = actor.TypeInfo.AIData.Move_CloseEnough;

      actor.AI.SetTarget(Target_Position);
      actor.AI.AdjustRotation(engine, actor);

      actor.AI.SetTargetSpeed(Target_Speed);
      actor.AI.AdjustSpeed(actor);

      float dist = engine.TrueVision.TVMathLibrary.GetDistanceVec3D(actor.GetGlobalPosition(), Target_Position);
      Complete |= (dist <= CloseEnoughDistance);
      Complete |= (ResumeTime < engine.Game.GameTime);
    }
  }
}
