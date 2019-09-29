using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.ActorTypes.Groups;
using SWEndor.AI;
using SWEndor.AI.Actions;
using SWEndor.Core;
using SWEndor.Models;
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

  public class WeaponInfo
  {
    public static readonly WeaponInfo[] NullArrayCache = new WeaponInfo[0];

    public WeaponInfo(string name, string weapproj)
    {
      Name = name;
      DisplayName = name;

      if (weapproj != null)
      {
        Projectile = (Projectile)Globals.Engine.ActorTypeFactory.Get(weapproj); //!
      }
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

      if (stat.WeaponProjectile != null)
        Projectile = (Projectile)engine.ActorTypeFactory.Get(stat.WeaponProjectile);

      Init();
    }

    public readonly string Name = "Null Weapon";
    public readonly string DisplayName = "null";

    private readonly Projectile Projectile = null; // cache
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

    // Player Config
    public bool RequirePlayerTargetLock = false;
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
      if (Ammo == MaxAmmo)
      {
        AmmoReloadCooldown = engine.Game.GameTime + AmmoReloadRate;
      }

      if (MaxAmmo > 0 && AmmoReloadCooldown < engine.Game.GameTime && Ammo < MaxAmmo)
      {
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

    public bool CanTarget(Engine engine, ActorInfo owner, ActorInfo target)
    {
      // player override
      if (owner.IsPlayer && !engine.PlayerInfo.PlayerAIEnabled)
        return true;

      // null
      if (target == null)
        return AIAttackNull;

      return (target.TypeInfo.AIData.TargetType & AIAttackTargets) != 0;
    }

    private bool CreateProjectile(Engine engine, ActorInfo owner, ActorInfo target)
    {
      if (owner == null)
        return false;

      if (owner.IsPlayer
        && !engine.PlayerInfo.PlayerAIEnabled 
        && (!RequirePlayerTargetLock || target != null))
      { // Player
        if (Projectile == null)
          return true;

        TV_3DVECTOR targetloc = GetFirePosition(owner);

        ActorCreationInfo acinfo = new ActorCreationInfo(Projectile);
        acinfo.Position = owner.GetRelativePositionXYZ(targetloc.x, targetloc.y, targetloc.z);

        if (owner.MoveData.Speed > Projectile.MoveLimitData.MinSpeed)
          acinfo.InitialSpeed = owner.MoveData.Speed;

        if (EnablePlayerAutoAim && target != null)
        {
          float dist = ActorDistanceInfo.GetDistance(owner, target);

          float d;
          if (AutoAimMaxDeviation == AutoAimMinDeviation)
            d = dist / Projectile.MoveLimitData.MaxSpeed * AutoAimMinDeviation;
          else
            d = dist / Projectile.MoveLimitData.MaxSpeed * (AutoAimMinDeviation + (AutoAimMaxDeviation - AutoAimMinDeviation) * (float)engine.Random.NextDouble());

          TV_3DVECTOR dir = new TV_3DVECTOR();
          ActorInfo a2 = target.ParentForCoords;
          if (a2 == null)
            dir = target.GetRelativePositionXYZ(0, 0, target.MoveData.Speed * d) - owner.GetGlobalPosition();
          else
            dir = target.GetGlobalPosition() + a2.GetRelativePositionXYZ(0, 0, a2.MoveData.Speed * d) - a2.GetGlobalPosition() - owner.GetGlobalPosition();

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
          a.QueueLast(new Wait(ProjectileWaitBeforeHoming));
          a.QueueLast(new ProjectileAttackActor(target));
          a.QueueLast(new Lock());
        }
        else // define CurrentAction.target
          a.QueueLast(new ProjectileAttackActor(target));

        return true;
      }

      if ((target == null && AIAttackNull)
          || (target != null && (target.TypeInfo.AIData.TargetType & AIAttackTargets) != 0)
          )
      { // AI 
        if (Projectile == null)
          return true;

        if (target != null && (owner.IsAggregateMode|| target.IsAggregateMode))
        {
          Projectile.FireAggregation(owner, target, Projectile);
        }

        TV_3DVECTOR targetloc = GetFirePosition(owner);

        ActorCreationInfo acinfo = new ActorCreationInfo(Projectile);
        acinfo.Position = owner.GetRelativePositionXYZ(targetloc.x, targetloc.y, targetloc.z);

        if (owner.MoveData.Speed > Projectile.MoveLimitData.MinSpeed)
          acinfo.InitialSpeed = owner.MoveData.Speed;

        if (EnableAIAutoAim && target != null)
        {
          float dist = ActorDistanceInfo.GetDistance(owner, target);

          float d;
          if (AutoAimMaxDeviation == AutoAimMinDeviation)
            d = dist / Projectile.MoveLimitData.MaxSpeed * AutoAimMinDeviation;
          else
            d = dist / Projectile.MoveLimitData.MaxSpeed * (AutoAimMinDeviation + (AutoAimMaxDeviation - AutoAimMinDeviation) * (float)engine.Random.NextDouble());

          TV_3DVECTOR dir = new TV_3DVECTOR();
          ActorInfo a2 = target.ParentForCoords;
          if (a2 == null)
            dir = target.GetRelativePositionXYZ(0, 0, target.MoveData.Speed * d) - owner.GetGlobalPosition();
          else
            dir = target.GetGlobalPosition() + a2.GetRelativePositionXYZ(0, 0, a2.MoveData.Speed * d) - a2.GetGlobalPosition() - owner.GetGlobalPosition();

          acinfo.Rotation = dir.ConvertDirToRot();
        }
        else
        {
          acinfo.Rotation = owner.GetGlobalRotation();
        }

        ActorInfo a = engine.ActorFactory.Create(acinfo);
        owner.AddChild(a);
        a.Faction = owner.Faction;

        if (a.Mask.Has(ComponentMask.HAS_AI))
        {
          a.QueueLast(new Wait(ProjectileWaitBeforeHoming));
          a.QueueLast(new ProjectileAttackActor(target));
          a.QueueLast(new Lock());
        }
        else // define CurrentAction.target
          a.QueueLast(new ProjectileAttackActor(target));

        return true;
      }
      return false;
    }
  }
}
