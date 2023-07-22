using MTV3D65;
using SWEndor.Game.Actors;
using SWEndor.Game.Actors.Models;
using SWEndor.Game.ActorTypes;
using SWEndor.Game.AI;
using SWEndor.Game.AI.Actions;
using SWEndor.Game.Core;
using SWEndor.Game.Models;
using SWEndor.Game.Projectiles;
using SWEndor.Game.ProjectileTypes;
using SWEndor.Game.Primitives.Extensions;

namespace SWEndor.Game.Weapons
{
  public class WeaponInfo
  {
    internal static readonly WeaponInfo[] NullArrayCache = new WeaponInfo[0];

    public WeaponInfo(string name)
    {
      Name = name;
      DisplayName = name;

      //Proj.Projectile = ProjectileTypeInfo.Null;
      //ActorProj = ActorTypeInfo.Null;
    }

    public readonly string Name = "Null Weapon";
    public readonly string DisplayName = "null";

    internal WeapAmmoInfo Ammo = WeapAmmoInfo.Default;
    internal WeapAimInfo Aim = WeapAimInfo.Default;
    internal WeapTgtInfo Targeter = WeapTgtInfo.Default;
    internal WeapProjInfo Proj = WeapProjInfo.Default;
    internal WeapPortInfo Port = WeapPortInfo.Default;


    public void Init()
    {
      if (Proj.Projectile != null)
        Targeter.Init(Proj.Projectile);
      else if (Proj.ActorProj != null)
        Targeter.Init(Proj.ActorProj);

      Port.Init();
    }

    private TV_3DVECTOR GetFirePosition(ActorInfo owner)
    {
      return Port.GetFirePos();
    }

    public void Reload(Engine engine)
    {
      Ammo.Reload(engine);
    }

    public bool Fire(Engine engine, ActorInfo owner, ActorInfo target, int burst)
    {
      int burstremaining = burst;
      bool fired = false;

      if (Port.Cooldown < engine.Game.GameTime && (Ammo.Count > 0 || Ammo.Max < 0))
      {
        Port.Cooldown = engine.Game.GameTime;
        while (burstremaining > 0)
        {
          if (CreateProjectile(engine, owner, target))
          {
            Port.Cooldown += Port.CooldownRate * burstremaining;
            if (Port.CooldownRateRandom != 0)
            {
              Port.Cooldown += (float)engine.Random.NextDouble() * Port.CooldownRateRandom;
            }
            fired = true;
            Port.Next();
            if (Ammo.Max > 0)
              Ammo.Count--;
          }
          burstremaining--;
        }
        ActorInfo p = engine.PlayerInfo.Actor;
        if (fired)
        {
          if (owner.IsPlayer)
          {
            if (Proj.FireSound.Length == 1)
              engine.SoundManager.SetSound(Proj.FireSound[0]);
            else if (Proj.FireSound.Length > 1)
              engine.SoundManager.SetSound(Proj.FireSound[engine.Random.Next(0, Proj.FireSound.Length)]);
          }
          else
            ActorTypes.Components.SoundSourceData.Play(engine, 0.75f, owner.GetGlobalPosition(), 750, Proj.FireSound, false, false);
        }
      }
      return fired;
    }

    public bool CanTarget(ActorInfo target)
    {
      if (target == null)
        return Targeter.AIAttackNull;
      else
        return Targeter.AIAttackTargets.Has(target.TargetType)
            && ((Targeter.AIExcludeTargets == TargetExclusionState.NONE) || (!target.TargetState.Intersects(Targeter.AIExcludeTargets)));
    }

    private bool CreateProjectile(Engine engine, ActorInfo owner, ActorInfo target, bool IsPlayer)
    {
      if (Proj.Projectile == null)
        return true;

      if (!IsPlayer
        && target != null
        && (owner.IsAggregateMode && target.IsAggregateMode))
      {
        owner.TypeInfo.FireAggregation(owner, target, Proj.Projectile);
        return true;
      }

      TV_3DVECTOR targetloc = GetFirePosition(owner);

      ProjectileCreationInfo acinfo = new ProjectileCreationInfo(Proj.Projectile);
      acinfo.Position = owner.GetRelativePositionXYZ(targetloc.x, targetloc.y, targetloc.z);

      if (owner.MoveData.Speed > Proj.Projectile.MoveLimitData.MinSpeed)
        acinfo.InitialSpeed = owner.MoveData.Speed;

      if ((IsPlayer ? Aim.EnablePlayerAutoAim : Aim.EnableAIAutoAim) && target != null)
      {
        float dist = DistanceModel.GetDistance(engine, owner, target);

        float d = (Aim.AutoAimMaxDeviation == Aim.AutoAimMinDeviation)
                ? dist / Proj.Projectile.MoveLimitData.MaxSpeed * Aim.AutoAimMinDeviation
                : dist / Proj.Projectile.MoveLimitData.MaxSpeed * (Aim.AutoAimMinDeviation + (Aim.AutoAimMaxDeviation - Aim.AutoAimMinDeviation) * (float)engine.Random.NextDouble());

        ActorInfo a2 = target.ParentForCoords;
        TV_3DVECTOR dir = (a2 == null)
                        ? target.GetRelativePositionXYZ(0, 0, target.MoveData.Speed * d) - owner.GetGlobalPosition()
                        : target.GetGlobalPosition() + a2.GetRelativePositionXYZ(0, 0, a2.MoveData.Speed * d) - a2.GetGlobalPosition() - owner.GetGlobalPosition();

        acinfo.Rotation = dir.ConvertDirToRot(engine.TrueVision.TVMathLibrary);
      }
      else
      {
        acinfo.Rotation = owner.GetGlobalRotation();
      }

      acinfo.OwnerID = owner.ID;
      acinfo.TargetID = target?.ID ?? -1;
      acinfo.LifeTime = Proj.LifeTime;
      ProjectileInfo a = engine.ProjectileFactory.Create(acinfo);
      return true;
    }

    private bool CreateProjectile(Engine engine, ActorInfo owner, ActorInfo target)
    {
      if (owner == null)
        return false;

      if (Proj.WeaponType == WeaponType.LASER && !owner.IsSystemOperational(SystemPart.LASER_WEAPONS))
        return false;
      if (Proj.WeaponType == WeaponType.ION && !owner.IsSystemOperational(SystemPart.ION_WEAPONS))
        return false;
      else if ((Proj.WeaponType == WeaponType.MISSILE || Proj.WeaponType == WeaponType.TORPEDO) && !owner.IsSystemOperational(SystemPart.PROJECTILE_LAUNCHERS))
        return false;


      if (owner.IsPlayer
        && !engine.PlayerInfo.PlayerAIEnabled
        && (!Targeter.RequirePlayerTargetLock || target != null))
      { // Player
        if (Proj.IsActor)
          return CreateActor(engine, owner, target, true);
        else
          return CreateProjectile(engine, owner, target, true);
      }

      if ((target == null && Targeter.AIAttackNull)
          || (target != null && target.TargetType.Intersects(Targeter.AIAttackTargets)
          ))
      { // AI 
        if (Proj.IsActor)
          return CreateActor(engine, owner, target, false);
        else
          return CreateProjectile(engine, owner, target, false);
      }
      return false;
    }

    private bool CreateActor(Engine engine, ActorInfo owner, ActorInfo target, bool IsPlayer)
    {
      if (Proj.ActorProj == null)
        return true;

      TV_3DVECTOR targetloc = GetFirePosition(owner);

      ActorCreationInfo acinfo = new ActorCreationInfo(Proj.ActorProj);
      acinfo.Position = owner.GetRelativePositionXYZ(targetloc.x, targetloc.y, targetloc.z);

      if (owner.MoveData.Speed > Proj.ActorProj.MoveLimitData.MinSpeed)
        acinfo.InitialSpeed = owner.MoveData.Speed;

      if (Aim.EnablePlayerAutoAim && target != null)
      {
        float dist = DistanceModel.GetDistance(engine, owner, target);

        float d = (Aim.AutoAimMaxDeviation == Aim.AutoAimMinDeviation)
                ? dist / Proj.ActorProj.MoveLimitData.MaxSpeed * Aim.AutoAimMinDeviation
                : dist / Proj.ActorProj.MoveLimitData.MaxSpeed * (Aim.AutoAimMinDeviation + (Aim.AutoAimMaxDeviation - Aim.AutoAimMinDeviation) * (float)engine.Random.NextDouble());

        ActorInfo a2 = target.ParentForCoords;
        TV_3DVECTOR dir = (a2 == null)
                        ? target.GetRelativePositionXYZ(0, 0, target.MoveData.Speed * d) - owner.GetGlobalPosition()
                        : target.GetGlobalPosition() + a2.GetRelativePositionXYZ(0, 0, a2.MoveData.Speed * d) - a2.GetGlobalPosition() - owner.GetGlobalPosition();

        acinfo.Rotation = dir.ConvertDirToRot(engine.TrueVision.TVMathLibrary);
      }
      else
      {
        acinfo.Rotation = owner.GetGlobalRotation();
      }

      ActorInfo a = engine.ActorFactory.Create(acinfo);
      a.Faction = owner.Faction;
      owner.AddChild(a);

      if (a.Mask.Has(ComponentMask.HAS_AI))
      {
        a.QueueLast(Wait.GetOrCreate(Proj.HomingDelay));
        a.QueueLast(ProjectileAttackActor.GetOrCreate(target));
        a.QueueLast(Lock.GetOrCreate());
      }
      else // define CurrentAction.target
      {
        a.QueueLast(ProjectileAttackActor.GetOrCreate(target));
      }

      if (Proj.LifeTime > 0)
        a.DyingTimerSet(Proj.LifeTime, true);
      return true;
    }
  }
}
