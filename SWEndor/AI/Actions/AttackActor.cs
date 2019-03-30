using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Types;
using SWEndor.Scenarios;
using SWEndor.Weapons;
using System;

namespace SWEndor.AI.Actions
{
  public class AttackActor : ActionInfo
  {
    public AttackActor(ActorInfo target, float follow_distance = -1, float too_close_distance = -1, bool can_interrupt = true, float hunt_interval = 15) : base("AttackActor")
    {
      Target_Actor = target;
      FollowDistance = follow_distance;
      TooCloseDistance = too_close_distance;
      CanInterrupt = can_interrupt;

      ReHuntTime = Game.Instance().GameTime + hunt_interval;
    }

    // parameters
    public ActorInfo Target_Actor = null;
    public TV_3DVECTOR Target_Position = new TV_3DVECTOR();
    public float FollowDistance = -1;
    public float TooCloseDistance = -1;
    public float SpeedAdjustmentDistanceRange = 250;
    private float ReHuntTime = 0;

    public override string ToString()
    {
      return string.Format("{0},{1},{2},{3},{4},{5},{6},{7}"
                          , Name
                          , (Target_Actor != null) ? Target_Actor.ID : -1
                          , FollowDistance
                          , TooCloseDistance
                          , SpeedAdjustmentDistanceRange
                          , ReHuntTime
                          , CanInterrupt
                          , Complete
                          );
    }

    public override void Process(ActorInfo owner)
    {
      if (Target_Actor == null)
      {
        Complete = true;
        return;
      }

      if (CheckBounds(owner))
      {
        if (FollowDistance < 0)
          FollowDistance = owner.TypeInfo.Move_CloseEnough;

        if (TooCloseDistance < 0)
          TooCloseDistance = 0.75f * owner.MovementInfo.Speed;

        float dist = ActorDistanceInfo.GetDistance(owner, Target_Actor);
        if (dist > TooCloseDistance)
        {
          float d = dist / Globals.LaserSpeed;
          ActorInfo a2 = Target_Actor.AttachToMesh > 0 ? Target_Actor.Parent : null;
          if (a2 == null)
          {
            Target_Position = Target_Actor.GetRelativePositionXYZ(0, 0, Target_Actor.MovementInfo.Speed * d);
          }
          else
          {
            Target_Position = a2.GetRelativePositionXYZ(Target_Actor.GetLocalPosition().x, Target_Actor.GetLocalPosition().y, Target_Actor.GetLocalPosition().z + a2.MovementInfo.Speed * d);
          }

          float delta_angle = AdjustRotation(owner, Target_Position, true);

          float addspd = (owner.MovementInfo.MaxSpeed > Target_Actor.MovementInfo.Speed) ? owner.MovementInfo.MaxSpeed - Target_Actor.MovementInfo.Speed : 0;
          float subspd = (owner.MovementInfo.MinSpeed < Target_Actor.MovementInfo.Speed) ? Target_Actor.MovementInfo.Speed - owner.MovementInfo.MinSpeed : 0;

          if (dist > FollowDistance)
            AdjustSpeed(owner, Target_Actor.MovementInfo.Speed + (dist - FollowDistance) / SpeedAdjustmentDistanceRange * addspd);
          else
            AdjustSpeed(owner, Target_Actor.MovementInfo.Speed - (FollowDistance - dist) / SpeedAdjustmentDistanceRange * subspd);

          WeaponInfo weapon = null;
          int burst = 0;
          owner.SelectWeapon(Target_Actor, delta_angle, dist, out weapon, out burst);
          if (weapon != null)
          {
            weapon.Fire(owner, Target_Actor, burst);
          }
          else
          {
            if (owner.TypeInfo.AggressiveTracker)
            {
              AggressiveTracking(owner);
            }
            
            if (owner.TypeInfo.NoMove) // can't move to you, I give up
            {
              Complete = true;
            }
          }

          if (CanInterrupt && ReHuntTime < Game.Instance().GameTime)
          {
            Complete = true;
            ActionManager.QueueNext(owner, new Hunt());
          }
          else
          {
            Complete |= (Target_Actor.CreationState != CreationState.ACTIVE || Target_Actor.ActorState == ActorState.DYING || Target_Actor.ActorState == ActorState.DEAD);
          }
        }
        else
        {
          if (Target_Actor.TypeInfo.TargetType.HasFlag(TargetType.FIGHTER))
          {
            float evadeduration = 2000 / (Target_Actor.GetTrueSpeed() + 500);
            ActionManager.QueueFirst(owner, new Evade(evadeduration));
          }
          else if (!(Target_Actor.TypeInfo is ProjectileGroup))
          {
            ActionManager.QueueFirst(owner, new Move(MakeAltPosition(owner, Target_Actor.GetTopParent()), owner.MovementInfo.MaxSpeed));
          }
          return;
        }

        if (CheckImminentCollision(owner, owner.MovementInfo.Speed * 2.5f))
        {
          ActionManager.QueueFirst(owner, new AvoidCollisionRotate(owner.ProspectiveCollisionImpact, owner.ProspectiveCollisionNormal));
          //if (owner.ProspectiveCollisionActor != null && owner.ProspectiveCollisionActor.GetTopParent() == Target_Actor.GetTopParent())
          //  ActionManager.QueueNext(owner, new Rotate(owner.ProspectiveCollisionImpact + owner.ProspectiveCollisionNormal * 10000, owner.MovementInfo.MinSpeed, 45));
        }
      }
    }

    private TV_3DVECTOR MakeAltPosition(ActorInfo owner, ActorInfo target)
    {
      float radius = 0;
      TV_3DVECTOR center = new TV_3DVECTOR();
      target.GetBoundingSphere(ref center, ref radius);
      center += target.GetPosition();
      radius += Engine.Instance().Random.Next((int)(-4 * owner.TypeInfo.MaxSpeed), (int)(4 * owner.TypeInfo.MaxSpeed));

      float xzAngle = Engine.Instance().Random.Next(0, 360);

      center.x += (float)Math.Cos(xzAngle) * radius;
      center.y += Engine.Instance().Random.Next(-200, 200);
      center.x += (float)Math.Sin(xzAngle) * radius;

      Utilities.Clamp(ref center, GameScenarioManager.Instance().MinAIBounds * 0.75f, GameScenarioManager.Instance().MaxAIBounds * 0.75f);
      
      return center;
    }

    private void AggressiveTracking(ActorInfo owner)
    {
      float dist = 0;
      float delta_angle = 0;

      foreach (ActorInfo a in ActorFactory.Instance().GetHoldingList())
      {
        if (a != null
            && owner != a
            && a.CreationState == CreationState.ACTIVE
            && a.ActorState != ActorState.DYING
            && a.ActorState != ActorState.DEAD
            && a.CombatInfo.IsCombatObject
            && !owner.Faction.IsAlliedWith(a.Faction))
        {
          dist = ActorDistanceInfo.GetDistance(a, owner, owner.GetWeaponRange());

          TV_3DVECTOR vec = new TV_3DVECTOR();
          TV_3DVECTOR dir = owner.GetDirection();
          Engine.Instance().TVMathLibrary.TVVec3Normalize(ref vec, a.GetPosition() - owner.GetPosition());
          delta_angle = Engine.Instance().TVMathLibrary.ACos(Engine.Instance().TVMathLibrary.TVVec3Dot(dir, vec));

          WeaponInfo weapon = null;
          int burst = 0;
          owner.SelectWeapon(a, delta_angle, dist, out weapon, out burst);
          if (weapon != null)
          {
            weapon.Fire(owner, a, burst);
            return;
          }
        }
      }
    }
  }
}
