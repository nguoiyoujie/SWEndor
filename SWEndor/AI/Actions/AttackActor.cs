using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Actors.Data;
using SWEndor.Actors.Traits;
using SWEndor.ActorTypes;
using SWEndor.Primitives;
using SWEndor.Weapons;
using System;

namespace SWEndor.AI.Actions
{
  public class AttackActor : ActionInfo
  {
    public AttackActor(ActorInfo targetActor, float follow_distance = -1, float too_close_distance = -1, bool can_interrupt = true, float hunt_interval = 15) : base("AttackActor")
    {
      Target_Actor = targetActor;
      FollowDistance = follow_distance;
      TooCloseDistance = too_close_distance;
      CanInterrupt = can_interrupt;

      ReHuntTime = Globals.Engine.Game.GameTime + hunt_interval; //!
    }

    // parameters
    public readonly ActorInfo Target_Actor = null;
    public TV_3DVECTOR Target_Position = new TV_3DVECTOR();
    public float FollowDistance = -1;
    public float TooCloseDistance = -1;
    public float SpeedAdjustmentDistanceRange = 250;
    private float ReHuntTime = 0;

    public override string ToString()
    {
      return "{0},{1},{2},{3},{4},{5},{6},{7}".F(Name
                                              , Target_Actor.ID
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
      ActorInfo target = Target_Actor;
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

        float dist = ActorDistanceInfo.GetDistance(actor, Target_Actor);
        if (dist > TooCloseDistance)
        {
          float d = dist / Globals.LaserSpeed;
          ActorInfo a2 = target.Relation.UseParentCoords ? target.Relation.Parent : null;
          if (a2 == null)
          {
            Target_Position = target.GetRelativePositionXYZ(0, 0, target.MoveData.Speed * d);
          }
          else
          {
            Target_Position = a2.GetRelativePositionXYZ(target.Transform.Position.x, target.Transform.Position.y, target.Transform.Position.z + a2.MoveData.Speed * d);
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
          actor.WeaponSystemInfo.SelectWeapon(Target_Actor, delta_angle, dist, out weapon, out burst);
          if (weapon != null)
          {
            weapon.Fire(actor, Target_Actor, burst);
          }
          else
          {
            if (actor.TypeInfo.AggressiveTracker)
            {
              AggressiveTracking(engine, actor);
            }
            
            if (!actor.StateModel.ComponentMask.Has(ComponentMask.CAN_MOVE)) // can't move to you, I give up
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
            Complete |= (!target.Active || target.StateModel.IsDyingOrDead);
          }
        }
        else
        {
          if (target.TypeInfo.TargetType.HasFlag(TargetType.FIGHTER))
          {
            float evadeduration = 2000 / (target.GetTrueSpeed() + 500);
            actor.QueueFirst(new Evade(evadeduration));
          }
          else if (!(target.TypeInfo is ActorTypes.Groups.Projectile))
          {
            actor.QueueFirst(new Move(MakeAltPosition(actor, target.TopParent), actor.MoveData.MaxSpeed));
          }
          return;
        }

        if (CheckImminentCollision(actor))
        {
          CollisionSystem.CreateAvoidAction(engine, actor);
        }
      }
    }

    private TV_3DVECTOR MakeAltPosition(ActorInfo actor, ActorInfo target)
    {
      float radius = 0;
      TV_3DVECTOR center = new TV_3DVECTOR();
      Engine engine = actor.Engine;
      if (target != null)
      {
        BoundingSphere sph = target.MeshModel.GetBoundingSphere(true); //engine.MeshDataSet.Mesh_getBoundingBox(actorID, true);

        //BoundingSphere sph = engine.MeshDataSet.Mesh_getBoundingSphere(targetActorID, true);
        center = sph.Position;
        radius = sph.Radius + engine.Random.Next((int)(-4 * actor.TypeInfo.MaxSpeed), (int)(4 * actor.TypeInfo.MaxSpeed));

        float xzAngle = engine.Random.Next(0, 360);

        center.x += (float)Math.Cos(xzAngle * Globals.PI / 180) * radius;
        center.y += engine.Random.Next(-200, 200);
        center.z += (float)Math.Sin(xzAngle * Globals.PI / 180) * radius;

        Utilities.Clamp(ref center, engine.GameScenarioManager.MinAIBounds * 0.75f, engine.GameScenarioManager.MaxAIBounds * 0.75f);
      }
      return center;
    }

    private void AggressiveTracking(Engine engine, ActorInfo actor)
    {
      float dist = 0;
      float delta_angle = 0;

      Action<Engine, ActorInfo> action = new Action<Engine, ActorInfo>(
        (_, a) =>
         {
           if (a != null
               && actor != a
               && a.Active
               && !a.StateModel.IsDyingOrDead
               && actor.CombatData.IsCombatObject
               && !actor.Faction.IsAlliedWith(a.Faction))
           {
             dist = ActorDistanceInfo.GetDistance(a, actor, actor.WeaponSystemInfo.GetWeaponRange());

             TV_3DVECTOR vec = new TV_3DVECTOR();
             TV_3DVECTOR dir = actor.Transform.GetGlobalDirection(actor);
             engine.TrueVision.TVMathLibrary.TVVec3Normalize(ref vec, a.GetGlobalPosition() - actor.GetGlobalPosition());
             delta_angle = engine.TrueVision.TVMathLibrary.ACos(engine.TrueVision.TVMathLibrary.TVVec3Dot(dir, vec));

             WeaponInfo weapon = null;
             int burst = 0;
             actor.WeaponSystemInfo.SelectWeapon(a, delta_angle, dist, out weapon, out burst);
             if (weapon != null)
             {
               weapon.Fire(actor, a, burst);
               return;
             }
           }
         }
      );

      engine.ActorFactory.DoEach(action);
    }
  }
}
