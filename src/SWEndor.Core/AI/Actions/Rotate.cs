using MTV3D65;
using Primrose.Primitives.Factories;
using SWEndor.Actors;
using SWEndor.Core;
using SWEndor.Primitives.Extensions;

namespace SWEndor.AI.Actions
{
  internal class Rotate : ActionInfo
  {

    private static ObjectPool<Rotate> _pool;
    static Rotate() { _pool = ObjectPool<Rotate>.CreateStaticPool(() => { return new Rotate(); }, (a) => { a.Reset(); }); }

    private Rotate() : base("Rotate") { CanInterrupt = false; }

    public static Rotate GetOrCreate(TV_3DVECTOR target_position, float speed, float close_enough_angle = 0.1f, bool can_interrupt = true)
    {
      Rotate h = _pool.GetNew();

      h.Target_Position = target_position;
      h.Target_Speed = speed;
      h.CloseEnoughAngle = close_enough_angle;
      h.CanInterrupt = can_interrupt;
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

    // parameters
    public TV_3DVECTOR Target_Position = new TV_3DVECTOR();
    public float Target_Speed = 0;
    public float CloseEnoughAngle = 0.1f;

    public override string ToString()
    {
      return string.Join(",", new string[]
      {
          Name
        , Target_Position.Str()
        , Target_Speed.ToString()
        , CloseEnoughAngle.ToString()
        , CanInterrupt.ToString()
        , Complete.ToString()
      });
    }

    public override void Process(Engine engine, ActorInfo actor)
    {
      if (actor.MoveData.MaxTurnRate == 0)
      {
        Complete = true;
        return;
      }

      if (CheckBounds(actor))
      {
        actor.AI.SetTarget(Target_Position);
        float delta_angle = actor.AI.AdjustRotation(engine, actor);

        actor.AI.SetTargetSpeed(Target_Speed);
        float delta_speed = actor.AI.AdjustSpeed(actor, false);

        Complete |= (delta_angle <= CloseEnoughAngle && delta_angle >= -CloseEnoughAngle && delta_speed == 0);
      }

      TV_3DVECTOR vNormal = new TV_3DVECTOR();
      TV_3DVECTOR vImpact = new TV_3DVECTOR();
      if (CheckImminentCollision(actor))
      {
        CreateAvoidAction(actor);
      }
    }
  }
}
