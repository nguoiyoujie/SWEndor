using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Models;
using SWEndor.ActorTypes;
using SWEndor.AI;
using SWEndor.AI.Actions;
using SWEndor.Core;
using SWEndor.Models;
using SWEndor.Projectiles;
using SWEndor.ProjectileTypes;
using SWEndor.Primitives.Extensions;

namespace SWEndor.Weapons
{
  public enum WeaponType
  {
    NONE,
    LASER,
    ION,
    MISSILE,
    TORPEDO
  }

  public enum TargetAcqType
  {
    NONE = -1,
    ANY = 0,
    ENEMIES = 1,
    FRIENDS = 2
  }

  public class WeaponInfo
  {
    internal static readonly WeaponInfo[] NullArrayCache = new WeaponInfo[0];

    public WeaponInfo(string name)
    {
      Name = name;
      DisplayName = name;

      // TO-DO: Dedicated Projectile class that need no casting 
      Projectile = ProjectileTypeInfo.Null;
      //ActorProj = ActorTypeInfo.Null;
    }

    public WeaponInfo(Engine engine, WeaponStatInfo stat)
    {
      Name = stat.Name ?? "Null";
      DisplayName = stat.DisplayName ?? "None";

      WeaponCooldown = stat.WeaponCooldown;
      WeaponCooldownRate = stat.WeaponCooldownRate;
      WeaponCooldownRateRandom = stat.WeaponCooldownRateRandom;

      Burst = stat.Burst;
      MaxAmmo = stat.MaxAmmo;
      Ammo = MaxAmmo;
      AmmoReloadCooldown = stat.AmmoReloadCooldown;
      AmmoReloadRate = stat.AmmoReloadRate;
      AmmoReloadRateRandom = stat.AmmoReloadRateRandom;
      AmmoReloadAmount = stat.AmmoReloadAmount;
      ProjectileWaitBeforeHoming = stat.ProjectileWaitBeforeHoming;

      FirePositions = stat.FirePositions;
      CurrentPositionIndex = stat.CurrentPositionIndex;

      // Auto Aim Bot
      EnablePlayerAutoAim = stat.EnablePlayerAutoAim;
      EnableAIAutoAim = stat.EnableAIAutoAim;
      AutoAimMinDeviation = stat.AutoAimMinDeviation;
      AutoAimMaxDeviation = stat.AutoAimMaxDeviation;

      // Player Config
      RequirePlayerTargetLock = stat.RequirePlayerTargetLock;
      Type = stat.Type;

      // AI Config
      AIAttackTargets = stat.AIAttackTargets;

      AIAttackNull = stat.AIAttackNull;

      AngularRange = stat.AngularRange;
      Range = stat.Range;

      FireSound = stat.FireSound;
      IsActor = stat.IsActor;

      if (stat.WeaponProjectile != null)
        if (stat.IsActor)
        {
          ActorProj = engine.ActorTypeFactory.Get(stat.WeaponProjectile);
          //Projectile = ProjectileTypeInfo.Null;
        }
        else
        {
          Projectile = engine.ProjectileTypeFactory.Get(stat.WeaponProjectile);
          //ActorProj = ActorTypeInfo.Null;
        }
      Init();
    }

    public readonly string Name = "Null Weapon";
    public readonly string DisplayName = "null";

    private readonly ProjectileTypeInfo Projectile = null; // cache
    private readonly ActorTypeInfo ActorProj = null; // cache
    public bool IsActor = false;
    public float WeaponCooldown = 0;
    public float WeaponCooldownRate = 1;
    public float WeaponCooldownRateRandom = 0;

    public int Burst = 1;
    public int Ammo = -1;
    public int MaxAmmo = -1;
    public float AmmoReloadCooldown = 1;
    public float AmmoReloadRate = 1;
    public float AmmoReloadRateRandom = 0;
    public int AmmoReloadAmount = 1;
    public float ProjectileWaitBeforeHoming = 0;

    public TV_3DVECTOR[] FirePositions = null;
    public TV_3DVECTOR[] UIFirePositions = null;

    public int CurrentPositionIndex = 0;

    // Auto Aim Bot
    public bool EnablePlayerAutoAim = false;
    public bool EnableAIAutoAim = false;
    public float AutoAimMinDeviation = 1;
    public float AutoAimMaxDeviation = 1;

    // Targeter
    public bool RequirePlayerTargetLock = false;
    public bool RequireAITargetLock = false; // TO-DO: Implement logic
    public float TargetLock_TimeRequired = 0; // TO-DO: Implement logic

    public TargetAcqType PlayerTargetAcqType = TargetAcqType.ANY; // TO-DO: Implement logic
    public TargetAcqType AITargetAcqType = TargetAcqType.ENEMIES; // TO-DO: Implement logic

    // Player Config
    public WeaponType Type = WeaponType.NONE;

    // AI Config
    public TargetType AIAttackTargets = TargetType.ANY;
    public bool AIAttackNull = true;
    public float AngularRange = 10;
    public float Range = 4500;

    // Misc
    public string[] FireSound = null;

    public void Init()
    {
      if (Range == 0 && Projectile != null)
        Range = Projectile.MoveLimitData.MaxSpeed * Projectile.TimedLifeData.TimedLife;

      UIFirePositions = new TV_3DVECTOR[FirePositions.Length];

      for (int i = 0; i < UIFirePositions.Length; i++)
      {
        float x = FirePositions[i].x;
        float y = FirePositions[i].y;

        if (x != 0 || y != 0)
        {
          float absX = (x > 0) ? x : -x;
          float absY = (y > 0) ? y : -y;

          if (absX > absY)
          {
            x *= 32 / absX;
            y *= 32 / absX;
          }
          else
          {
            x *= 32 / absY;
            y *= 32 / absY;
          }
        }
        UIFirePositions[i].x = x;
        UIFirePositions[i].y = y;
      }
    }

    private TV_3DVECTOR GetFirePosition(ActorInfo owner)
    {
      if (FirePositions.Length == 0)
        return new TV_3DVECTOR(0, 0, 0);

      if (CurrentPositionIndex >= FirePositions.Length)
      {
        CurrentPositionIndex = 0;
      }
      return FirePositions[CurrentPositionIndex];
    }

    public void Reload(Engine engine)
    {
      if (MaxAmmo > 0 && AmmoReloadCooldown < engine.Game.GameTime && Ammo < MaxAmmo)
      {
        if (Ammo == MaxAmmo)
        {
          AmmoReloadCooldown = engine.Game.GameTime + AmmoReloadRate;
        }

        AmmoReloadCooldown = engine.Game.GameTime + AmmoReloadRate;
        if (AmmoReloadRateRandom != 0)
        {
          AmmoReloadCooldown += (float)engine.Random.NextDouble() * AmmoReloadRateRandom;
        }
        Ammo += AmmoReloadAmount;
        if (Ammo > MaxAmmo)
          Ammo = MaxAmmo;
      }
    }

    public bool Fire(Engine engine, ActorInfo owner, ActorInfo target, int burst)
    {
      int burstremaining = burst;
      bool fired = false;

      if (WeaponCooldown < engine.Game.GameTime && (Ammo > 0 || MaxAmmo < 0))
      {
        WeaponCooldown = engine.Game.GameTime;
        while (burstremaining > 0)
        {
          if (CreateProjectile(engine, owner, target))
          {
            WeaponCooldown += WeaponCooldownRate * burstremaining;
            if (WeaponCooldownRateRandom != 0)
            {
              WeaponCooldown += (float)engine.Random.NextDouble() * WeaponCooldownRateRandom * 4;
            }
            fired = true;
            CurrentPositionIndex++;
            if (MaxAmmo > 0)
              Ammo--;
          }
          burstremaining--;
        }
        ActorInfo p = engine.PlayerInfo.Actor;
        if (fired)
        {
          if (owner.IsPlayer)
          {
            if (FireSound.Length == 1)
              engine.SoundManager.SetSound(FireSound[0]);
            else if (FireSound.Length > 1)
              engine.SoundManager.SetSound(FireSound[engine.Random.Next(0, FireSound.Length)]);
          }

          ActorTypes.Components.SoundSourceData.Play(engine, 1f, owner.GetGlobalPosition(), 750, FireSound, false, false);
        }
      }
      return fired;
    }

    public bool CanTarget(ActorInfo owner, ActorInfo target)
    {
      return (target == null) ? AIAttackNull : (target.TypeInfo.AIData.TargetType & AIAttackTargets) != 0;
    }

    private bool CreateProjectile(Engine engine, ActorInfo owner, ActorInfo target, bool IsPlayer)
    {
      if (Projectile == null)
        return true;

      if (!IsPlayer
        && target != null
        && (owner.IsAggregateMode && target.IsAggregateMode))
      {
        owner.TypeInfo.FireAggregation(owner, target, Projectile);
        return true;
      }

      TV_3DVECTOR targetloc = GetFirePosition(owner);

      ProjectileCreationInfo acinfo = new ProjectileCreationInfo(Projectile);
      acinfo.Position = owner.GetRelativePositionXYZ(targetloc.x, targetloc.y, targetloc.z);

      if (owner.MoveData.Speed > Projectile.MoveLimitData.MinSpeed)
        acinfo.InitialSpeed = owner.MoveData.Speed;

      if ((IsPlayer ? EnablePlayerAutoAim : EnableAIAutoAim) && target != null)
      {
        float dist = DistanceModel.GetDistance(engine, owner, target);

        float d = (AutoAimMaxDeviation == AutoAimMinDeviation)
                ? dist / Projectile.MoveLimitData.MaxSpeed * AutoAimMinDeviation
                : dist / Projectile.MoveLimitData.MaxSpeed * (AutoAimMinDeviation + (AutoAimMaxDeviation - AutoAimMinDeviation) * (float)engine.Random.NextDouble());

        ActorInfo a2 = target.ParentForCoords;
        TV_3DVECTOR dir = (a2 == null)
                        ? target.GetRelativePositionXYZ(0, 0, target.MoveData.Speed * d) - owner.GetGlobalPosition()
                        : target.GetGlobalPosition() + a2.GetRelativePositionXYZ(0, 0, a2.MoveData.Speed * d) - a2.GetGlobalPosition() - owner.GetGlobalPosition();

        acinfo.Rotation = dir.ConvertDirToRot();
      }
      else
      {
        acinfo.Rotation = owner.GetGlobalRotation();
      }

      ProjectileInfo a = engine.ProjectileFactory.Create(acinfo);
      a.OwnerID = owner?.ID ?? -1;
      a.TargetID = target?.ID ?? -1;
      return true;
    }

    private bool CreateProjectile(Engine engine, ActorInfo owner, ActorInfo target)
    {
      if (owner == null)
        return false;

      if ((Type == WeaponType.LASER || Type == WeaponType.ION)
        && owner.TypeInfo.SystemData.AllowSystemDamage && owner.GetStatus(SystemPart.LASER_WEAPONS) != SystemState.ACTIVE)
        return false;
      else if ((Type == WeaponType.MISSILE || Type == WeaponType.TORPEDO)
        && owner.TypeInfo.SystemData.AllowSystemDamage && owner.GetStatus(SystemPart.PROJECTILE_LAUNCHERS) != SystemState.ACTIVE)
        return false;


      if (owner.IsPlayer
        && !engine.PlayerInfo.PlayerAIEnabled
        && (!RequirePlayerTargetLock || target != null))
      { // Player
        if (IsActor)
          return CreateActor(engine, owner, target, true);
        else
          return CreateProjectile(engine, owner, target, true);
      }

      if ((target == null && AIAttackNull)
          || (target != null && (target.TypeInfo.AIData.TargetType & AIAttackTargets) != 0)
          )
      { // AI 
        if (IsActor)
          return CreateActor(engine, owner, target, false);
        else
          return CreateProjectile(engine, owner, target, false);
      }
      return false;
    }

    private bool CreateActor(Engine engine, ActorInfo owner, ActorInfo target, bool IsPlayer)
    {
      if (ActorProj == null)
        return true;

      TV_3DVECTOR targetloc = GetFirePosition(owner);

      ActorCreationInfo acinfo = new ActorCreationInfo(ActorProj);
      acinfo.Position = owner.GetRelativePositionXYZ(targetloc.x, targetloc.y, targetloc.z);

      if (owner.MoveData.Speed > ActorProj.MoveLimitData.MinSpeed)
        acinfo.InitialSpeed = owner.MoveData.Speed;

      if (EnablePlayerAutoAim && target != null)
      {
        float dist = DistanceModel.GetDistance(engine, owner, target);

        float d = (AutoAimMaxDeviation == AutoAimMinDeviation)
                ? dist / ActorProj.MoveLimitData.MaxSpeed * AutoAimMinDeviation
                : dist / ActorProj.MoveLimitData.MaxSpeed * (AutoAimMinDeviation + (AutoAimMaxDeviation - AutoAimMinDeviation) * (float)engine.Random.NextDouble());

        ActorInfo a2 = target.ParentForCoords;
        TV_3DVECTOR dir = (a2 == null)
                        ? target.GetRelativePositionXYZ(0, 0, target.MoveData.Speed * d) - owner.GetGlobalPosition()
                        : target.GetGlobalPosition() + a2.GetRelativePositionXYZ(0, 0, a2.MoveData.Speed * d) - a2.GetGlobalPosition() - owner.GetGlobalPosition();

        acinfo.Rotation = dir.ConvertDirToRot();
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
        a.QueueLast(Wait.GetOrCreate(ProjectileWaitBeforeHoming));
        a.QueueLast(ProjectileAttackActor.GetOrCreate(target));
        a.QueueLast(Lock.GetOrCreate());
      }
      else // define CurrentAction.target
      {
        a.QueueLast(ProjectileAttackActor.GetOrCreate(target));
      }
      return true;
    }
  }
}
