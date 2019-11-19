using MTV3D65;
using Primrose.Primitives.Factories;
using SWEndor.Actors;
using SWEndor.Core;
using SWEndor.Models;
using SWEndor.Primitives.Extensions;

namespace SWEndor.AI.Actions
{
  internal class Move : ActionInfo
  {
    internal static int _count = 0;
    private static ObjectPool<Move> _pool;
    static Move() { _pool = ObjectPool<Move>.CreateStaticPool(() => { return new Move(); }, (a) => { a.Reset(); }); }

    private Move() : base("Move") { }

    public static Move GetOrCreate(TV_3DVECTOR target_position, float speed, float close_enough_distance = -1, bool can_interrupt = true)
    {
      Move h = _pool.GetNew();
      _count++;
      h.Target_Position = target_position;
      h.Target_Speed = speed;
      h.CloseEnoughDistance = close_enough_distance;
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
      _count--;
    }

    // parameters
    public TV_3DVECTOR Target_Position = new TV_3DVECTOR();
    public float Target_Speed = 0;
    public float CloseEnoughDistance = -1;

    public override string ToString()
    {
      return string.Join(",", new string[]
      {
          Name
        , Target_Position.Str()
        , Target_Speed.ToString()
        , CloseEnoughDistance.ToString()
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

      if (CheckBounds(actor))
      {
        if (CloseEnoughDistance < 0)
          CloseEnoughDistance = actor.TypeInfo.AIData.Move_CloseEnough;

        actor.AI.SetTarget(Target_Position);
        actor.AI.AdjustRotation(engine, actor);

        actor.AI.SetTargetSpeed(Target_Speed);
        actor.AI.AdjustSpeed(actor);

        float dist = engine.TrueVision.TVMathLibrary.GetDistanceVec3D(actor.GetGlobalPosition(), Target_Position);
        Complete |= (dist <= CloseEnoughDistance);
      }

      TV_3DVECTOR vNormal = new TV_3DVECTOR();
      TV_3DVECTOR vImpact = new TV_3DVECTOR();
      if (CheckImminentCollision(actor))
      {
        CreateAvoidAction(actor);
      }
      else
      {
        foreach (ActorInfo l in actor.Squad.Members)
        {
          if (l != null && actor != l && DistanceModel.GetRoughDistance(actor, l) < l.MoveData.Speed * 0.5f)
          {
            actor.QueueFirst(Evade.GetOrCreate(0.5f));
            break;
          }
          else if (actor == l)
            break;
        }
      }
    }
  }
}
