﻿using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Actors.Data;
using SWEndor.ActorTypes;
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

    public override void Process(Engine engine, int actorID)
    {
      ActorInfo actor = engine.ActorFactory.Get(actorID);
      ActorInfo target = engine.ActorFactory.Get(Target_ActorID);
      if (target == null)
      {
        Complete = true;
        return;
      }

      if (CheckBounds(actor))
      {
        if (FollowDistance < 0)
          FollowDistance = actor.TypeInfo.Move_CloseEnough;

        if (TooCloseDistance < 0)
          TooCloseDistance = 0.75f * actor.MoveData.Speed;

        float dist = ActorDistanceInfo.GetDistance(actorID, Target_ActorID);
        if (dist > TooCloseDistance)
        {
          float d = dist / Globals.LaserSpeed;
          ActorInfo a2 = target.AttachToParent ? engine.ActorFactory.Get(target.ParentID) : null;
          if (a2 == null)
          {
            Target_Position = target.GetRelativePositionXYZ(0, 0, target.MoveData.Speed * d);
          }
          else
          {
            Target_Position = a2.GetRelativePositionXYZ(target.GetLocalPosition().x, target.GetLocalPosition().y, target.GetLocalPosition().z + a2.MoveData.Speed * d);
          }

          float delta_angle = AdjustRotation(actor, Target_Position, true);

          float addspd = (actor.MoveData.MaxSpeed > target.MoveData.Speed) ? actor.MoveData.MaxSpeed - target.MoveData.Speed : 0;
          float subspd = (actor.MoveData.MinSpeed < target.MoveData.Speed) ? target.MoveData.Speed - actor.MoveData.MinSpeed : 0;

          if (dist > FollowDistance)
            AdjustSpeed(actor, target.MoveData.Speed + (dist - FollowDistance) / SpeedAdjustmentDistanceRange * addspd);
          else
            AdjustSpeed(actor, target.MoveData.Speed - (FollowDistance - dist) / SpeedAdjustmentDistanceRange * subspd);

          WeaponInfo weapon = null;
          int burst = 0;
          actor.WeaponSystemInfo.SelectWeapon(Target_ActorID, delta_angle, dist, out weapon, out burst);
          if (weapon != null)
          {
            weapon.Fire(engine, actorID, Target_ActorID, burst);
          }
          else
          {
            if (actor.TypeInfo.AggressiveTracker)
            {
              AggressiveTracking(engine, actorID);
            }
            
            if (!engine.MaskDataSet[actorID].Has(ComponentMask.CAN_MOVE)) // can't move to you, I give up
            {
              Complete = true;
            }
          }

          if (CanInterrupt && ReHuntTime < engine.Game.GameTime)
          {
            Complete = true;
            engine.ActionManager.QueueNext(actorID, new Hunt());
          }
          else
          {
            Complete |= (target.CreationState != CreationState.ACTIVE || target.ActorState.IsDyingOrDead());
          }
        }
        else
        {
          if (target.TypeInfo.TargetType.HasFlag(TargetType.FIGHTER))
          {
            float evadeduration = 2000 / (target.GetTrueSpeed() + 500);
            engine.ActionManager.QueueFirst(actorID, new Evade(evadeduration));
          }
          else if (!(target.TypeInfo is ActorTypes.Groups.Projectile))
          {
            engine.ActionManager.QueueFirst(actorID, new Move(MakeAltPosition(engine, actorID, target.TopParent), actor.MoveData.MaxSpeed));
          }
          return;
        }

        if (CheckImminentCollision(actor, actor.MoveData.Speed * 2.5f))
        {
          CollisionSystem.CreateAvoidAction(engine, actorID);
          //if (owner.ProspectiveCollisionActor != null && owner.ProspectiveCollisionActor.TopParent == Target_Actor.TopParent)
          //  ActionManager.QueueNext(owner, new Rotate(owner.ProspectiveCollision.Impact + owner.CollisionInfo.ProspectiveCollision.Normal * 10000, owner.MovementInfo.MinSpeed, 45));
        }
      }
    }

    private TV_3DVECTOR MakeAltPosition(Engine engine, int actorID, int targetActorID)
    {
      float radius = 0;
      TV_3DVECTOR center = new TV_3DVECTOR();
      ActorInfo actor = engine.ActorFactory.Get(actorID);
      ActorInfo target = engine.ActorFactory.Get(targetActorID);
      if (target != null)
      {
        target.GetBoundingSphere(ref center, ref radius);
        center += target.GetPosition();
        radius += engine.Random.Next((int)(-4 * actor.TypeInfo.MaxSpeed), (int)(4 * actor.TypeInfo.MaxSpeed));

        float xzAngle = engine.Random.Next(0, 360);

        center.x += (float)Math.Cos(xzAngle) * radius;
        center.y += engine.Random.Next(-200, 200);
        center.x += (float)Math.Sin(xzAngle) * radius; // x??

        Utilities.Clamp(ref center, engine.GameScenarioManager.MinAIBounds * 0.75f, engine.GameScenarioManager.MaxAIBounds * 0.75f);
      }
      return center;
    }

    private void AggressiveTracking(Engine engine, int actorID)
    {
      float dist = 0;
      float delta_angle = 0;
      ActorInfo actor = engine.ActorFactory.Get(actorID);

      Action<Engine, int> action = new Action<Engine, int>(
        (_, aID) =>
         {
           ActorInfo a = engine.ActorFactory.Get(aID);
           if (a != null
               && actorID != aID
               && a.CreationState == CreationState.ACTIVE
               && a.ActorState != ActorState.DYING
               && a.ActorState != ActorState.DEAD
               && engine.ActorDataSet.CombatData[actor.dataID].IsCombatObject
               && !actor.Faction.IsAlliedWith(a.Faction))
           {
             dist = ActorDistanceInfo.GetDistance(aID, actorID, actor.WeaponSystemInfo.GetWeaponRange());

             TV_3DVECTOR vec = new TV_3DVECTOR();
             TV_3DVECTOR dir = actor.GetDirection();
             engine.TrueVision.TVMathLibrary.TVVec3Normalize(ref vec, a.GetPosition() - actor.GetPosition());
             delta_angle = engine.TrueVision.TVMathLibrary.ACos(engine.TrueVision.TVMathLibrary.TVVec3Dot(dir, vec));

             WeaponInfo weapon = null;
             int burst = 0;
             actor.WeaponSystemInfo.SelectWeapon(aID, delta_angle, dist, out weapon, out burst);
             if (weapon != null)
             {
               weapon.Fire(engine, actorID, aID, burst);
               return;
             }
           }
         }
      );

      engine.ActorFactory.DoEach(action);
    }
  }
}
