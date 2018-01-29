using MTV3D65;
using System;
using System.Collections.Generic;
using System.Text;

namespace SWEndor.Actions
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
      if (CheckBounds(owner))
      {
        if (FollowDistance < 0)
          FollowDistance = owner.TypeInfo.Move_CloseEnough;

        if (TooCloseDistance < 0)
          TooCloseDistance = 0.5f * owner.Speed;

        float dist = ActorDistanceInfo.GetDistance(owner, Target_Actor);
        if (dist > TooCloseDistance)
        {
          float d = dist / Globals.LaserSpeed;
          ActorInfo a2 = ActorFactory.Instance().GetActor(Target_Actor.AttachToMesh);
          if (a2 == null)
          {
            Target_Position = Target_Actor.GetRelativePositionXYZ(0, 0, Target_Actor.Speed * d);
          }
          else
          {
            //Target_Position = Target_Actor.GetPosition() - owner.GetPosition();
            Target_Position = a2.GetRelativePositionXYZ(Target_Actor.GetLocalPosition().x, Target_Actor.GetLocalPosition().y, Target_Actor.GetLocalPosition().z + a2.Speed * d);
          }

          //Target_Position = Target_Actor.GetRelativePositionXYZ(0, 0, Target_Actor.GetTrueSpeed() * (d + Game.Instance().TimeSinceRender));

          float delta_angle = AdjustRotation(owner, Target_Position, true);

          float addspd = (owner.MaxSpeed > Target_Actor.Speed) ? owner.MaxSpeed - Target_Actor.Speed : 0;
          float subspd = (owner.MinSpeed < Target_Actor.Speed) ? Target_Actor.Speed - owner.MinSpeed : 0;

          if (dist > FollowDistance)
            AdjustSpeed(owner, Target_Actor.Speed + (dist - FollowDistance) / SpeedAdjustmentDistanceRange * addspd);
          else
            AdjustSpeed(owner, Target_Actor.Speed - (FollowDistance - dist) / SpeedAdjustmentDistanceRange * subspd);

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
          if (Target_Actor.TypeInfo is FighterGroup || Target_Actor.TypeInfo is TIEGroup)
          {
            float evadeduration = 2000 / (Target_Actor.GetTrueSpeed() + 500);
            ActionManager.QueueFirst(owner, new Evade(evadeduration));
          }
          else
          {
            ActionManager.QueueFirst(owner, new Move(MakeAltPosition(owner), owner.MaxSpeed));
          }
          return;
        }

        TV_3DVECTOR vNormal = new TV_3DVECTOR();
        TV_3DVECTOR vImpact = new TV_3DVECTOR();
        if (CheckImminentCollision(owner, FollowDistance, out vImpact, out vNormal))
        {
          ActionManager.QueueFirst(owner, new AvoidCollisionRotate(vImpact, vNormal));
        }
      }
    }

    private TV_3DVECTOR MakeAltPosition(ActorInfo owner)
    {
      float x = owner.GetPosition().x + Engine.Instance().Random.Next((int)(-4 * owner.TypeInfo.MaxSpeed), (int)(4 * owner.TypeInfo.MaxSpeed));
      if ((int)(4 * owner.TypeInfo.MaxSpeed) + owner.GetPosition().x > GameScenarioManager.Instance().MaxBounds.x * 0.75f)
      {
        x -= (int)(4 * owner.TypeInfo.MaxSpeed) + owner.GetPosition().x - GameScenarioManager.Instance().MaxBounds.x * 0.75f;
      }
      else if ((int)(-4 * owner.TypeInfo.MaxSpeed) + owner.GetPosition().x < GameScenarioManager.Instance().MinBounds.x * 0.75f)
      {
        x += (int)(-4 * owner.TypeInfo.MaxSpeed) + owner.GetPosition().x - GameScenarioManager.Instance().MinBounds.x * 0.75f;
      }

      float y = Engine.Instance().Random.Next(-200, 200);

      float z = owner.GetPosition().z + Engine.Instance().Random.Next((int)(-4f * owner.TypeInfo.MaxSpeed), (int)(4f * owner.TypeInfo.MaxSpeed));
      if ((int)(4f * owner.TypeInfo.MaxSpeed) + owner.GetPosition().z > GameScenarioManager.Instance().MaxBounds.z * 0.75f)
      {
        z -= (int)(4f * owner.TypeInfo.MaxSpeed) + owner.GetPosition().z - GameScenarioManager.Instance().MaxBounds.z * 0.75f;
      }
      else if ((int)(-4f * owner.TypeInfo.MaxSpeed) + owner.GetPosition().z < GameScenarioManager.Instance().MinBounds.z * 0.75f)
      {
        z += (int)(-4f * owner.TypeInfo.MaxSpeed) + owner.GetPosition().z - GameScenarioManager.Instance().MinBounds.z * 0.75f;
      }

      return new TV_3DVECTOR(x, y, z);
    }

    private void AggressiveTracking(ActorInfo owner)
    {
      float dist = 0;
      float angle_x = 0;
      float angle_y = 0;
      float delta_angle = 0;

      foreach (ActorInfo a in ActorFactory.Instance().GetActorList())
      {
        if (a != null
            && owner != a
            && a.CreationState == CreationState.ACTIVE
            && a.ActorState != ActorState.DYING
            && a.ActorState != ActorState.DEAD
            && a.IsCombatObject
            && !owner.Faction.IsAlliedWith(a.Faction))
        {
          dist = ActorDistanceInfo.GetDistance(a, owner, owner.GetWeaponRange());
          TV_3DVECTOR rot = owner.GetRotation();
          TV_3DVECTOR tgtrot = Utilities.GetRotation(a.GetPosition() - owner.GetPosition());

          angle_x = tgtrot.x - rot.x;
          angle_y = tgtrot.y - rot.y;

          while (angle_x < -180)
            angle_x += 360;

          while (angle_x > 180)
            angle_x -= 360;

          while (angle_y < -180)
            angle_y += 360;

          while (angle_y > 180)
            angle_y -= 360;

          delta_angle = angle_x + angle_y;

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
