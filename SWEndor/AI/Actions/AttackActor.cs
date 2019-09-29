using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.ActorTypes;
using SWEndor.Core;
using SWEndor.Primitives;
using SWEndor.Primitives.Extensions;
using SWEndor.Weapons;
using System;

namespace SWEndor.AI.Actions
{
  public class AttackActor : ActionInfo
  {
    public AttackActor(int targetActorID, float follow_distance = -1, float too_close_distance = -1, bool can_interrupt = true, float hunt_interval = 15) : base("AttackActor")
    {
      Target_ActorID = targetActorID;
      FollowDistance = follow_distance;
      TooCloseDistance = too_close_distance;
      CanInterrupt = can_interrupt;

      ReHuntTime = Globals.Engine.Game.GameTime + hunt_interval; //!
    }

    // parameters
    public readonly int Target_ActorID = -1;
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
      if (target == null || target.Faction.IsAlliedWith(actor.Faction))
      {
        Complete = true;
        return;
      }

      if (!CheckBounds(actor))
        return;

      actor.AIData.SetTarget(engine, actor, target, true);
      actor.AIData.SetFollowDistance(actor, FollowDistance);

      if (TooCloseDistance < 0)
        TooCloseDistance = actor.MoveData.Speed * 0.75f;

      float dist = actor.AIData.GetDistanceToTargetActor(engine, actor);

      if (dist > TooCloseDistance)
      {
        float delta_angle = actor.AIData.AdjustRotation(engine, actor);
        actor.AIData.AdjustSpeed(actor);

        WeaponShotInfo w;
        actor.WeaponDefinitions.SelectWeapon(engine, actor, target, delta_angle, dist, out w);
        if (!w.IsNull)
        {
          w.Fire(engine, actor, target);
        }
        else
        {
          if (actor.TypeInfo.AIData.AggressiveTracker)
            AggressiveTracking(engine, actor.ID);

          if (!actor.Mask.Has(ComponentMask.CAN_MOVE)) // can't move to you, I give up
            Complete = true;
        }

        if (CanInterrupt && ReHuntTime < engine.Game.GameTime)
        {
          Complete = true;
          actor.QueueNext(new Hunt());
        }
        else
          Complete |= (!target.Active || target.IsDyingOrDead);
      }
      else
      {
        if (target.TypeInfo.AIData.TargetType.Has(TargetType.FIGHTER))
        {
          float evadeduration = 2000 / (target.GetTrueSpeed() + 500);
          actor.QueueFirst(new Evade(evadeduration));
        }
        else if (!(target.TypeInfo is ActorTypes.Groups.Projectile))
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
        ActorInfo leader = actor.Squad.Leader;
        if (leader != null && actor != leader && ActorDistanceInfo.GetRoughDistance(actor, leader) < leader.MoveData.Speed * 0.25f)
        {
          actor.QueueFirst(new Evade(0.25f));
        }
      }
    }

    private TV_3DVECTOR MakeAltPosition(Engine engine, ActorInfo actor, ActorInfo target)
    {
      float radius = 0;
      TV_3DVECTOR center = new TV_3DVECTOR();

      if (target != null)
      {
        BoundingSphere sph = target.GetBoundingSphere(true);
        center = sph.Position;
        radius = sph.Radius + engine.Random.Next((int)(-4 * actor.TypeInfo.MoveLimitData.MaxSpeed), (int)(4 * actor.TypeInfo.MoveLimitData.MaxSpeed));

        float xzAngle = engine.Random.Next(0, 360);

        center.x += (float)Math.Cos(xzAngle * Globals.PI / 180) * radius;
        center.y += engine.Random.Next(-200, 200);
        center.z += (float)Math.Sin(xzAngle * Globals.PI / 180) * radius;

        TV3DVecExts.Clamp(ref center, engine.GameScenarioManager.MinAIBounds * 0.75f, engine.GameScenarioManager.MaxAIBounds * 0.75f);
      }
      return center;
    }


    private Func<Engine, ActorInfo, ActorInfo, bool> aggressiveTracking = new Func<Engine, ActorInfo, ActorInfo, bool>(
      (e, a, c) =>
      {
        if (a != null
              && c.ID != a.ID
              && a.Active
              && !a.IsDyingOrDead
              && c.CombatData.IsCombatObject
              && !c.Faction.IsAlliedWith(a.Faction))
        {
          float dist = ActorDistanceInfo.GetDistance(a, c, c.WeaponDefinitions.GetWeaponRange());

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

    private void AggressiveTracking(Engine engine, int actorID)
    {
      ActorInfo actor = engine.ActorFactory.Get(actorID);
      engine.ActorFactory.DoUntil(aggressiveTracking, actor);
    }
  }
}
