using MTV3D65;
using SWEndor.Actors;
using SWEndor.Core;
using SWEndor.Models;
using Primrose.Primitives.Factories;
using Primrose.Primitives.Geometry;
using SWEndor.Weapons;
using System;
using SWEndor.Primitives.Extensions;

namespace SWEndor.AI.Actions
{
  internal class AttackActor : ActionInfo
  {
    internal static int _count = 0;
    internal static ObjectPool<AttackActor> _pool = new ObjectPool<AttackActor>(() => { return new AttackActor(); }, (a) => { a.Reset(); });

    private AttackActor() : base("AttackActor") { }

    public static AttackActor GetOrCreate(int targetActorID, float follow_distance = -1, float too_close_distance = -1, bool can_interrupt = true, float hunt_interval = 15)
    {
      AttackActor h = _pool.GetNew();
      _count++;
      h.Target_ActorID = targetActorID;
      h.FollowDistance = follow_distance;
      h.TooCloseDistance = too_close_distance;
      h.CanInterrupt = can_interrupt;
      h.ReHuntTime = Globals.Engine.Game.GameTime + hunt_interval; //! Global usage
      h.IsDisposed = false;
      return h;
    }

    // parameters
    public int Target_ActorID = -1;
    public TV_3DVECTOR Target_Position = new TV_3DVECTOR();
    public float FollowDistance = -1;
    public float TooCloseDistance = -1;
    public float SpeedAdjustmentDistanceRange = 250;
    private float ReHuntTime = 0;

    public override string ToString()
    {
      return string.Join(",", new string[]
        {
          Name
        , Target_ActorID.ToString()
        , FollowDistance.ToString()
        , TooCloseDistance.ToString()
        , SpeedAdjustmentDistanceRange.ToString()
        , ReHuntTime.ToString()
        , CanInterrupt.ToString()
        , Complete.ToString()
        });
    }

    public override void Process(Engine engine, ActorInfo actor)
    {
      ActorInfo target = engine.ActorFactory.Get(Target_ActorID);
      if (target == null || target.IsAlliedWith(actor))
      {
        Complete = true;
        return;
      }

      if (!CheckBounds(actor))
        return;

      actor.AI.SetTarget(engine, actor, target, true);
      actor.AI.SetFollowDistance(actor, FollowDistance);

      if (TooCloseDistance < 0)
        TooCloseDistance = actor.MoveData.Speed * 0.75f;

      float dist = actor.AI.GetDistanceToTargetActor(engine, actor);

      if (dist > TooCloseDistance)
      {
        float delta_angle = actor.AI.AdjustRotation(engine, actor);
        actor.AI.AdjustSpeed(actor);

        WeaponShotInfo w;
        actor.WeaponDefinitions.SelectWeapon(engine, actor, target, delta_angle, dist, out w);
        if (!w.IsNull)
        {
          w.Fire(engine, actor, target);
        }
        else
        {
          if (actor.TypeInfo.AIData.AggressiveTracker)
            AggressiveTracking(engine, actor);

          if (!actor.Mask.Has(ComponentMask.CAN_MOVE)) // can't move to you, I give up
            Complete = true;
        }

        if (CanInterrupt && ReHuntTime < engine.Game.GameTime)
        {
          Complete = true;
          actor.QueueNext(Hunt.GetOrCreate());
        }
        else
          Complete |= (!target.Active || target.IsDyingOrDead);
      }
      else
      {
        if (target.TypeInfo.AIData.TargetType.Has(TargetType.FIGHTER))
        {
          float evadeduration = 2000 / (target.MoveData.Speed + 500);
          actor.QueueFirst(Evade.GetOrCreate(evadeduration));
        }
        else if (!(target.TypeInfo.AIData.TargetType.Intersects(TargetType.MUNITION)))
        {
          actor.QueueFirst(new Move(MakeAltPosition(engine, actor, target.Parent), actor.MoveData.MaxSpeed));
        }
      }

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

    private static TV_3DVECTOR MakeAltPosition(Engine engine, ActorInfo actor, ActorInfo target)
    {
      float radius = 0;
      TV_3DVECTOR center = new TV_3DVECTOR();

      if (target != null)
      {
        Sphere sph = target.GetBoundingSphere(true);
        center = sph.Position;
        radius = sph.R + engine.Random.Next((int)(-4 * actor.TypeInfo.MoveLimitData.MaxSpeed), (int)(4 * actor.TypeInfo.MoveLimitData.MaxSpeed));

        float xzAngle = engine.Random.Next(0, 360);

        center.x += (float)Math.Cos(xzAngle * Globals.Deg2Rad) * radius;
        center.y += engine.Random.Next(-200, 200);
        center.z += (float)Math.Sin(xzAngle * Globals.Deg2Rad) * radius;

        TV3DVecExts.Clamp(ref center, engine.GameScenarioManager.MinAIBounds * 0.75f, engine.GameScenarioManager.MaxAIBounds * 0.75f);
      }
      return center;
    }


    private static Func<Engine, ActorInfo, ActorInfo, bool> aggressiveTracking = new Func<Engine, ActorInfo, ActorInfo, bool>(
      (e, a, c) =>
      {
        if (a != null
              && c.ID != a.ID
              && a.Active
              && !a.IsDyingOrDead
              && c.InCombat
              && !c.IsAlliedWith(a))
        {
          float dist = DistanceModel.GetDistance(e, a, c, c.WeaponDefinitions.GetWeaponRange());

          TV_3DVECTOR vec = new TV_3DVECTOR();
          TV_3DVECTOR dir = c.Direction;
          e.TrueVision.TVMathLibrary.TVVec3Normalize(ref vec, a.GetGlobalPosition() - c.GetGlobalPosition());
          float delta_angle = e.TrueVision.TVMathLibrary.ACos(e.TrueVision.TVMathLibrary.TVVec3Dot(dir, vec));

          WeaponShotInfo w;
          c.WeaponDefinitions.SelectWeapon(e, c, a, delta_angle, dist, out w);
          if (!w.IsNull)
          {
            w.Fire(e, c, a);
            return false;
          }
        }
        return true;
      }
    );

    private static void AggressiveTracking(Engine engine, ActorInfo actor)
    {
      engine.ActorFactory.DoUntil(aggressiveTracking, actor);
    }

    public override void Reset()
    {
      base.Reset();
      Target_ActorID = -1;
      Target_Position = new TV_3DVECTOR();
      FollowDistance = -1;
      TooCloseDistance = -1;
      SpeedAdjustmentDistanceRange = 250;
      ReHuntTime = 0;
    }

    public override void Return()
    {
      base.Return();
      _pool.Return(this);
      _count--;
    }
  }
}
