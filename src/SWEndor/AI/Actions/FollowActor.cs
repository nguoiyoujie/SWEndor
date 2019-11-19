using MTV3D65;
using Primrose.Primitives.Factories;
using SWEndor.Actors;
using SWEndor.Core;
using SWEndor.Models;

namespace SWEndor.AI.Actions
{
  internal class FollowActor : ActionInfo
  {
    internal static int _count = 0;
    private static ObjectPool<FollowActor> _pool;
    static FollowActor() { _pool = ObjectPool<FollowActor>.CreateStaticPool(() => { return new FollowActor(); }, (a) => { a.Reset(); }); }

    private FollowActor() : base("FollowActor") { }

    public static FollowActor GetOrCreate(int targetActorID, float follow_distance = 500, bool can_interrupt = true)
    {
      FollowActor h = _pool.GetNew();
      _count++;
      h.Target_ActorID = targetActorID;
      h.FollowDistance = follow_distance;
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
    public int Target_ActorID = -1;
    public float FollowDistance = 500;
    public float SpeedAdjustmentDistanceRange = 100;

    public override string ToString()
    {
      return string.Join(",", new string[]
      {
          Name
        , Target_ActorID.ToString()
        , FollowDistance.ToString()
        , SpeedAdjustmentDistanceRange.ToString()
        , CanInterrupt.ToString()
        , Complete.ToString()
      });
    }

    public override void Process(Engine engine, ActorInfo actor)
    {
      ActorInfo target = engine.ActorFactory.Get(Target_ActorID);
      if (target == null || actor.MoveData.MaxSpeed == 0)
      {
        Complete = true;
        return;
      }

      actor.AI.SetTarget(engine, actor, target, false);

      if (CheckBounds(actor))
      {
        actor.AI.AdjustRotation(engine, actor);
        float dist = DistanceModel.GetDistance(engine, actor, target, FollowDistance + 1);

        actor.AI.AdjustSpeed(actor);
        Complete |= (!target.Active);
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
