using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.ActorTypes.Group;
using SWEndor.AI.Actions;

namespace SWEndor.Weapons
{
  public class WeaponInfo
  {
    public WeaponInfo(string name, string weapproj)
    {
      Name = name;

      if (weapproj != null)
      {
        Projectile = (Projectile)Globals.Engine.ActorTypeFactory.Get(weapproj); //!
      }
    }

    public WeaponInfo(Engine engine, WeaponStatInfo stat)
    {
      Name = stat.Name;

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

    //public string WeaponProjectile = null;
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

    // AI Config
    public TargetType AIAttackTargets = TargetType.ANY;

    public bool AIAttackNull = true;

    public float AngularRange = 10;
    public float Range = 4500; //5500;


    // 
    public string FireSound = "laser_sf";

    public void Init()
    {
      if (Range == 0 && Projectile != null)
        Range = Projectile.MaxSpeed * Projectile.TimedLife;

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
      //return (FirePositions[CurrentPositionIndex] >= owner.WeaponPositions.Count) ? owner.WeaponPositions[0] : owner.WeaponPositions[FirePositions[CurrentPositionIndex]];
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

    public bool Fire(Engine engine, int ownerActorID, int targetActorID, int burst)
    {
      int burstremaining = burst;
      bool fired = false;

      if (WeaponCooldown < engine.Game.GameTime && (Ammo > 0 || MaxAmmo < 0))
      {
        WeaponCooldown = engine.Game.GameTime;
        while (burstremaining > 0)
        {
          if (CreateProjectile(engine, ownerActorID, targetActorID))
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
        if (fired && ActorInfo.IsPlayer(engine, ownerActorID))
        {
          engine.SoundManager.SetSound(FireSound);
        }
      }
      return fired;
    }

    public bool CanTarget(Engine engine, int ownerActorID, int targetActorID)
    {
      // player override
      if (ActorInfo.IsPlayer(engine, ownerActorID) && !engine.PlayerInfo.PlayerAIEnabled)
        return true;

      // null
      if (targetActorID < 0)
        return AIAttackNull;

      ActorInfo ta = engine.ActorFactory.Get(targetActorID);
      if (ta != null)
        return (ta.TypeInfo.TargetType & AIAttackTargets) != 0;
      return false;
    }

    private bool CreateProjectile(Engine engine, int ownerActorID, int targetActorID)
    {
      ActorInfo owner = engine.ActorFactory.Get(ownerActorID);
      if (owner == null)
        return false;

      ActorInfo target = engine.ActorFactory.Get(targetActorID);

      if ((ActorInfo.IsPlayer(engine, ownerActorID) 
        && !engine.PlayerInfo.PlayerAIEnabled 
        && (!RequirePlayerTargetLock || target != null)))
      { // Player
        if (Projectile == null)
          return true;

        TV_3DVECTOR targetloc = GetFirePosition(owner);

        ActorCreationInfo acinfo = new ActorCreationInfo(Projectile);
        acinfo.Position = owner.GetRelativePositionXYZ(targetloc.x, targetloc.y, targetloc.z);
        acinfo.Faction = owner.Faction;

        if (EnablePlayerAutoAim && target != null)
        {
          float dist = ActorDistanceInfo.GetDistance(ownerActorID, targetActorID);
          float d = dist / Projectile.MaxSpeed * (AutoAimMinDeviation + (AutoAimMaxDeviation - AutoAimMinDeviation) * (float)engine.Random.NextDouble());

          TV_3DVECTOR dir = target.GetRelativePositionXYZ(0, 0, target.GetTrueSpeed() * d) - owner.GetPosition();
          acinfo.Rotation = Utilities.GetRotation(dir);
        }
        else
        {
          acinfo.Rotation = owner.Rotation;
        }

        ActorInfo a = ActorInfo.Create(engine.ActorFactory, acinfo);
        a.Faction = owner.Faction;
        a.AddParent(ownerActorID);

        if (!a.TypeInfo.NoAI)
          engine.ActionManager.QueueLast(a.ID, new Wait(ProjectileWaitBeforeHoming));
        engine.ActionManager.QueueLast(a.ID, new AttackActor(targetActorID, 0, 0, false, 9999));
        engine.ActionManager.QueueLast(a.ID, new Idle());

        return true;
      }

      if ((target == null && AIAttackNull)
          || (target != null && (target.TypeInfo.TargetType & AIAttackTargets) != 0)
          )
      { // AI 
        if (Projectile == null)
          return true;

        if (target != null && (ActorInfo.IsAggregateMode(engine, ownerActorID) || ActorInfo.IsAggregateMode(engine, targetActorID)))
        {
          Projectile.FireAggregation(ownerActorID, targetActorID, Projectile);
        }

        TV_3DVECTOR targetloc = GetFirePosition(owner);

        ActorCreationInfo acinfo = new ActorCreationInfo(Projectile);
        acinfo.Position = owner.GetRelativePositionXYZ(targetloc.x, targetloc.y, targetloc.z);

        if (EnableAIAutoAim && target != null)
        {
          float dist = ActorDistanceInfo.GetDistance(ownerActorID, targetActorID);

          float d;
          if (AutoAimMaxDeviation == AutoAimMinDeviation)
          {
            d = dist / Projectile.MaxSpeed * AutoAimMinDeviation;
          }
          else
          {
            d = dist / Projectile.MaxSpeed * (AutoAimMinDeviation + (AutoAimMaxDeviation - AutoAimMinDeviation) * (float)engine.Random.NextDouble());
          }

          TV_3DVECTOR dir = new TV_3DVECTOR();
          ActorInfo a2 = target.AttachToParent ? engine.ActorFactory.Get(target.ParentID) : null;
          if (a2 == null)
          {
            dir = target.GetRelativePositionXYZ(0, 0, target.MoveComponent.Speed * d) - owner.GetPosition();
          }
          else
          {
            //dir = target.GetPosition() - owner.GetPosition();
            dir = a2.GetRelativePositionXYZ(target.GetLocalPosition().x, target.GetLocalPosition().y, target.GetLocalPosition().z + a2.MoveComponent.Speed * d) - owner.GetPosition();
          }

          acinfo.Rotation = Utilities.GetRotation(dir);
        }
        else
        {
          acinfo.Rotation = owner.GetRotation();
        }

        ActorInfo a = ActorInfo.Create(engine.ActorFactory, acinfo);
        a.AddParent(ownerActorID);
        a.Faction = owner.Faction;

        if (!a.TypeInfo.NoAI)
          engine.ActionManager.QueueLast(a.ID, new Wait(ProjectileWaitBeforeHoming));

        engine.ActionManager.QueueLast(a.ID, new AttackActor(targetActorID, 0, 0, false, 9999));
        engine.ActionManager.QueueLast(a.ID, new Lock());

        return true;
      }
      return false;
    }
  }
}
