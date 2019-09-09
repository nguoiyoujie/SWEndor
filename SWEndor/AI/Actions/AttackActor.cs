using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.ActorTypes;
using SWEndor.Primitives;
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
      return string.Format("{0},{1},{2},{3},{4},{5},{6},{7}"
                          , Name
                          , Target_ActorID
                          , FollowDistance
                          , TooCloseDistance
                          , SpeedAdjustmentDistanceRange
                          , ReHuntTime
                          , CanInterrupt
                          , Complete
                          );
    }

    public override void Process(Engine engine, ActorInfo actor)
    {
      ActorInfo target = engine.ActorFactory.Get(Target_ActorID);
      if (target == null || target.Faction.IsAlliedWith(actor.Faction))
      {
        Complete = true;
        return;
      }

      if (CheckBounds(actor))
      {
        if (FollowDistance < 0)
          FollowDistance = actor.TypeInfo.AIData.Move_CloseEnough;

        if (TooCloseDistance < 0)
          TooCloseDistance = 0.75f * actor.MoveData.Speed;

        float dist = ActorDistanceInfo.GetDistance(actor, target);
        if (dist > TooCloseDistance)
        {
          float d = dist / Globals.LaserSpeed + engine.Game.TimeSinceRender;
          ActorInfo a2 = target.ParentForCoords;
          if (a2 == null)
            Target_Position = target.GetRelativePositionXYZ(0, 0, target.MoveData.Speed * d);
          else
            Target_Position = target.GetGlobalPosition() + a2.GetRelativePositionXYZ(0, 0, a2.MoveData.Speed * d) - a2.GetGlobalPosition();

          float delta_angle = AdjustRotation(actor, Target_Position, true);

          float addspd = (actor.MoveData.MaxSpeed > target.MoveData.Speed) ? actor.MoveData.MaxSpeed - target.MoveData.Speed : 0;
          float subspd = (actor.MoveData.MinSpeed < target.MoveData.Speed) ? target.MoveData.Speed - actor.MoveData.MinSpeed : 0;

          if (dist > FollowDistance)
            AdjustSpeed(actor, target.MoveData.Speed + (dist - FollowDistance) / SpeedAdjustmentDistanceRange * addspd);
          else
            AdjustSpeed(actor, target.MoveData.Speed - (FollowDistance - dist) / SpeedAdjustmentDistanceRange * subspd);

          WeaponShotInfo w;
          actor.WeaponSystemInfo.SelectWeapon(engine, actor, target, delta_angle, dist, out w);
          if (!w.IsNull)
          {
            w.Fire(engine, actor, target);
          }
          else
          {
            if (actor.TypeInfo.AIData.AggressiveTracker)
            {
              AggressiveTracking(engine, actor.ID);
            }
            
            if (!engine.MaskDataSet[actor].Has(ComponentMask.CAN_MOVE)) // can't move to you, I give up
            {
              Complete = true;
            }
          }

          if (CanInterrupt && ReHuntTime < engine.Game.GameTime)
          {
            Complete = true;
            actor.QueueNext(new Hunt());
          }
          else
          {
            Complete |= (!target.Active || target.IsDyingOrDead);
          }
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

        if (CheckImminentCollision(actor, actor.MoveData.Speed * 2.5f))
        {
          CollisionSystem.CreateAvoidAction(engine, actor);
          //if (owner.ProspectiveCollisionActor != null && owner.ProspectiveCollisionActor.TopParent == Target_Actor.TopParent)
          //  ActionManager.QueueNext(owner, new Rotate(owner.ProspectiveCollision.Impact + owner.CollisionInfo.ProspectiveCollision.Normal * 10000, owner.MovementInfo.MinSpeed, 45));
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

        Utilities.Clamp(ref center, engine.GameScenarioManager.MinAIBounds * 0.75f, engine.GameScenarioManager.MaxAIBounds * 0.75f);
      }
      return center;
    }

    private void AggressiveTracking(Engine engine, int actorID)
    {
      float dist = 0;
      float delta_angle = 0;
      ActorInfo actor = engine.ActorFactory.Get(actorID);

      Func<Engine, ActorInfo, bool> fn = new Func<Engine, ActorInfo, bool>(
        (_, a) =>
         {
           if (a != null
               && actorID != a.ID
               && a.Active
               && !a.IsDyingOrDead
               && engine.ActorDataSet.CombatData[actor.dataID].IsCombatObject
               && !actor.Faction.IsAlliedWith(a.Faction))
           {
             dist = ActorDistanceInfo.GetDistance(a, actor, actor.WeaponSystemInfo.GetWeaponRange());

             TV_3DVECTOR vec = new TV_3DVECTOR();
             TV_3DVECTOR dir = actor.Direction;
             engine.TrueVision.TVMathLibrary.TVVec3Normalize(ref vec, a.GetGlobalPosition() - actor.GetGlobalPosition());
             delta_angle = engine.TrueVision.TVMathLibrary.ACos(engine.TrueVision.TVMathLibrary.TVVec3Dot(dir, vec));

             WeaponShotInfo w;
             actor.WeaponSystemInfo.SelectWeapon(engine, actor, a, delta_angle, dist, out w);
             if (!w.IsNull)
             {
               w.Fire(engine, actor, a);
               return false;
             }
           }
           return true;
         }
      );

      engine.ActorFactory.DoUntil(fn);
    }
  }
}
