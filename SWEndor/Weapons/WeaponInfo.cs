using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.AI;
using SWEndor.AI.Actions;
using SWEndor.Player;
using SWEndor.Sound;

namespace SWEndor.Weapons
{
  public class WeaponInfo
  {
    public WeaponInfo(string name)
    {
      Name = name;
      if (WeaponProjectile != null)
      {
        Range = WeaponProjectile.MaxSpeed * WeaponProjectile.TimedLife;
      }
    }

    public WeaponInfo(WeaponStatInfo stat)
    {
      Name = stat.Name;

      WeaponProjectile = (ProjectileGroup) ActorTypeInfo.Factory.Get(stat.WeaponProjectile);
      WeaponCooldown = stat.WeaponCooldown;
      WeaponCooldownRate = stat.WeaponCooldownRate;
      WeaponCooldownRateRandom = stat.WeaponCooldownRateRandom;

      Burst = stat.Burst;
      Ammo = stat.Ammo;
      MaxAmmo = stat.MaxAmmo;
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

      // AI Config
      AIAttackTargets = stat.AIAttackTargets;

      AIAttackNull = stat.AIAttackNull;

      AngularRange = stat.AngularRange;
      //Range = stat.Range;

      FireSound = stat.FireSound;
      if (WeaponProjectile != null)
        Range = WeaponProjectile.MaxSpeed * WeaponProjectile.TimedLife;
    }

    public readonly string Name = "Null Weapon";

    public ProjectileGroup WeaponProjectile = null;
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
    public int CurrentPositionIndex = 0;

    // Auto Aim Bot
    public bool EnablePlayerAutoAim = false;
    public bool EnableAIAutoAim = false;
    public float AutoAimMinDeviation = 1;
    public float AutoAimMaxDeviation = 1;

    // Player Config
    public bool RequirePlayerTargetLock = false;

    // AI Config
    public TargetType AIAttackTargets = TargetType.ANY;

    public bool AIAttackNull = true;

    public float AngularRange = 10;
    public float Range = 4500; //5500;


    // 
    public string FireSound = "laser_sf";

    private TV_3DVECTOR GetFirePosition(ActorInfo owner)
    {
      if (FirePositions.Length == 0)
        return new TV_3DVECTOR(0, 0, 0);

      if (CurrentPositionIndex >= FirePositions.Length)
      {
        CurrentPositionIndex = 0;
      }
      return FirePositions[CurrentPositionIndex];
      //return (FirePositions[CurrentPositionIndex] >= owner.WeaponPositions.Count) ? owner.WeaponPositions[0] : owner.WeaponPositions[FirePositions[CurrentPositionIndex]];
    }

    public void Reload()
    {
      if (Ammo == MaxAmmo)
      {
        AmmoReloadCooldown = Game.Instance().GameTime + AmmoReloadRate;
      }

      if (MaxAmmo > 0 && AmmoReloadCooldown < Game.Instance().GameTime && Ammo < MaxAmmo)
      {
        AmmoReloadCooldown = Game.Instance().GameTime + AmmoReloadRate;
        if (AmmoReloadRateRandom != 0)
        {
          AmmoReloadCooldown += (float)Engine.Instance().Random.NextDouble() * AmmoReloadRateRandom;
        }
        Ammo += AmmoReloadAmount;
      }
    }

    public bool Fire(ActorInfo owner, ActorInfo target, int burst)
    {
      int burstremaining = burst;
      bool fired = false;

      if (WeaponCooldown < Game.Instance().GameTime && (Ammo > 0 || MaxAmmo < 0))
      {
        WeaponCooldown = Game.Instance().GameTime;
        while (burstremaining > 0)
        {
          if (CreateProjectile(owner, target))
          {
            WeaponCooldown += WeaponCooldownRate * burstremaining;
            if (WeaponCooldownRateRandom != 0)
            {
              WeaponCooldown += (float)Engine.Instance().Random.NextDouble() * WeaponCooldownRateRandom * 4;
            }
            fired = true;
            CurrentPositionIndex++;
            if (MaxAmmo > 0)
              Ammo--;
          }
          burstremaining--;
        }
        if (fired && owner.IsPlayer())
        {
          SoundManager.Instance().SetSound(FireSound);
        }
      }
      return fired;
    }

    public bool CanTarget(ActorInfo owner, ActorInfo target)
    {
      return ((owner.IsPlayer() && !PlayerInfo.Instance().PlayerAIEnabled)
          || (target == null && AIAttackNull)
          || (target != null && (target.TypeInfo.TargetType & AIAttackTargets) != 0));
    }

    private bool CreateProjectile(ActorInfo owner, ActorInfo target)
    {
      if ((owner == PlayerInfo.Instance().Actor && !PlayerInfo.Instance().PlayerAIEnabled && (!RequirePlayerTargetLock || PlayerInfo.Instance().AimTarget != null)))
      { // Player
        if (WeaponProjectile == null)
          return true;

        target = PlayerInfo.Instance().AimTarget;

        TV_3DVECTOR targetloc = GetFirePosition(owner);

        ActorCreationInfo acinfo = new ActorCreationInfo(WeaponProjectile);
        acinfo.Position = owner.GetRelativePositionXYZ(targetloc.x, targetloc.y, targetloc.z);
        acinfo.Faction = owner.Faction;

        if (EnablePlayerAutoAim && target != null)
        {
          float dist = ActorDistanceInfo.GetDistance(owner, target);
          float d = dist / WeaponProjectile.MaxSpeed * (AutoAimMinDeviation + (AutoAimMaxDeviation - AutoAimMinDeviation) * (float)Engine.Instance().Random.NextDouble());

          TV_3DVECTOR dir = target.GetRelativePositionXYZ(0, 0, target.GetTrueSpeed() * d) - owner.GetPosition();
          acinfo.Rotation = Utilities.GetRotation(dir);
        }
        else
        {
          acinfo.Rotation = owner.Rotation;
        }

        ActorInfo a = ActorInfo.Create(acinfo);
        a.AddParent(owner);

        if (!a.TypeInfo.NoAI)
          ActionManager.QueueLast(a, new Wait(ProjectileWaitBeforeHoming));
        ActionManager.QueueLast(a, new AttackActor(target, 0, 0, false, 9999));
        ActionManager.QueueLast(a, new Idle());

        return true;
      }
      if ((target == null && AIAttackNull)
          || (target != null && (target.TypeInfo.TargetType & AIAttackTargets) != 0)
          )
      { // AI 
        if (WeaponProjectile == null)
          return true;

        if (target != null && (owner.IsAggregateMode() || target.IsAggregateMode()))
        {
          WeaponProjectile.FireAggregation(owner, target, WeaponProjectile);
        }

        TV_3DVECTOR targetloc = GetFirePosition(owner);

        ActorCreationInfo acinfo = new ActorCreationInfo(WeaponProjectile);
        acinfo.Position = owner.GetRelativePositionXYZ(targetloc.x, targetloc.y, targetloc.z);

        if (EnableAIAutoAim && target != null)
        {
          float dist = ActorDistanceInfo.GetDistance(owner, target);

          float d;
          if (AutoAimMaxDeviation == AutoAimMinDeviation)
          {
            d = dist / WeaponProjectile.MaxSpeed * AutoAimMinDeviation;
          }
          else
          {
            d = dist / WeaponProjectile.MaxSpeed * (AutoAimMinDeviation + (AutoAimMaxDeviation - AutoAimMinDeviation) * (float)Engine.Instance().Random.NextDouble());
          }

          TV_3DVECTOR dir = new TV_3DVECTOR();
          ActorInfo a2 = target.AttachToParent ? ActorInfo.Factory.Get(target.ParentID) : null;
          if (a2 == null)
          {
            dir = target.GetRelativePositionXYZ(0, 0, target.MovementInfo.Speed * d) - owner.GetPosition();
          }
          else
          {
            //dir = target.GetPosition() - owner.GetPosition();
            dir = a2.GetRelativePositionXYZ(target.GetLocalPosition().x, target.GetLocalPosition().y, target.GetLocalPosition().z + a2.MovementInfo.Speed * d) - owner.GetPosition();
          }

          acinfo.Rotation = Utilities.GetRotation(dir);
        }
        else
        {
          acinfo.Rotation = owner.GetRotation();
        }

        ActorInfo a = ActorInfo.Create(acinfo);
        a.AddParent(owner);

        if (!a.TypeInfo.NoAI)
          ActionManager.QueueLast(a, new Wait(ProjectileWaitBeforeHoming));
        ActionManager.QueueLast(a, new AttackActor(target, 0, 0, false, 9999));
        ActionManager.QueueLast(a, new Lock());

        return true;
      }
      return false;
    }
  }
}
