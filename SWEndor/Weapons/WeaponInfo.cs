using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.ActorTypes.Groups;
using SWEndor.AI;
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
        Range = Projectile.MaxSpeed * Projectile.TimedLifeData.TimedLife;

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

    public bool Fire(ActorInfo owner, ActorInfo target, int burst)
    {
      int burstremaining = burst;
      bool fired = false;

      if (WeaponCooldown < owner.Game.GameTime && (Ammo > 0 || MaxAmmo < 0))
      {
        WeaponCooldown = owner.Game.GameTime;
        while (burstremaining > 0)
        {
          if (CreateProjectile(owner, target))
          {
            WeaponCooldown += WeaponCooldownRate * burstremaining;
            if (WeaponCooldownRateRandom != 0)
            {
              WeaponCooldown += (float)owner.Engine.Random.NextDouble() * WeaponCooldownRateRandom * 4;
            }
            fired = true;
            CurrentPositionIndex++;
            if (MaxAmmo > 0)
              Ammo--;
          }
          burstremaining--;
        }

        if (fired && owner.IsPlayer)
        {
          owner.SoundManager.SetSound(FireSound);
        }
      }
      return fired;
    }

    public bool CanTarget(ActorInfo owner, ActorInfo target)
    {
      // player override
      if (owner.IsPlayer && !owner.PlayerInfo.PlayerAIEnabled)
        return true;

      // null
      if (target == null)
        return AIAttackNull;

      return (target.TypeInfo.TargetType & AIAttackTargets) != 0;
    }

    private bool CreateProjectile(ActorInfo owner, ActorInfo target)
    {
      if (owner == null)
        return false;

      Engine engine = owner.Engine;

      if ((owner.IsPlayer 
        && !engine.PlayerInfo.PlayerAIEnabled 
        && (!RequirePlayerTargetLock || target != null)))
      { // Player
        if (Projectile == null)
          return true;

        TV_3DVECTOR targetloc = GetFirePosition(owner);

        ActorCreationInfo acinfo = new ActorCreationInfo(Projectile);
        acinfo.Position = owner.GetRelativePositionXYZ(targetloc.x, targetloc.y, targetloc.z);

        if (owner.MoveData.Speed > Projectile.MinSpeed)
          acinfo.InitialSpeed = owner.MoveData.Speed;

        if (EnablePlayerAutoAim && target != null)
        {
          float dist = ActorDistanceInfo.GetDistance(owner, target);
          float d = dist / Projectile.MaxSpeed * (AutoAimMinDeviation + (AutoAimMaxDeviation - AutoAimMinDeviation) * (float)engine.Random.NextDouble());

          TV_3DVECTOR dir = target.GetGlobalPosition() - owner.GetGlobalPosition();
          ActorInfo tgtparent = target.Relation.UseParentCoords ? target.TopParent : null;
          if (tgtparent != null)
            dir += tgtparent.GetRelativePositionXYZ(0, 0, target.GetTrueSpeed() * d) - tgtparent.GetGlobalPosition();

          //TV_3DVECTOR dir = target.GetRelativePositionXYZ(0, 0, target.GetTrueSpeed() * d) - owner.GetPosition();
          acinfo.Rotation = Utilities.GetRotation(dir);
        }
        else
        {
          acinfo.Rotation = owner.Transform.Rotation;
        }

        ActorInfo a = engine.ActorFactory.Create(acinfo);
        a.Faction = owner.Faction;
        owner.AddChild(a);

        if (a.StateModel.ComponentMask.Has(ComponentMask.HAS_AI))
          a.QueueLast(new Wait(ProjectileWaitBeforeHoming));
        a.QueueLast(new AttackActor(target, 0, 0, false, 9999));
        a.QueueLast(new Idle());

        return true;
      }

      if ((target == null && AIAttackNull)
          || (target != null && (target.TypeInfo.TargetType & AIAttackTargets) != 0)
          )
      { // AI 
        if (Projectile == null)
          return true;

        if (target != null && (owner.IsAggregateMode || target.IsAggregateMode))
        {
          Projectile.FireAggregation(owner, target, Projectile);
        }

        TV_3DVECTOR targetloc = GetFirePosition(owner);

        ActorCreationInfo acinfo = new ActorCreationInfo(Projectile);
        acinfo.Position = owner.GetRelativePositionXYZ(targetloc.x, targetloc.y, targetloc.z);

        if (owner.MoveData.Speed > Projectile.MinSpeed)
          acinfo.InitialSpeed = owner.MoveData.Speed;

        if (EnableAIAutoAim && target != null)
        {
          float dist = ActorDistanceInfo.GetDistance(owner, target);

          float d;
          if (AutoAimMaxDeviation == AutoAimMinDeviation)
          {
            d = dist / Projectile.MaxSpeed * AutoAimMinDeviation;
          }
          else
          {
            d = dist / Projectile.MaxSpeed * (AutoAimMinDeviation + (AutoAimMaxDeviation - AutoAimMinDeviation) * (float)engine.Random.NextDouble());
          }

          TV_3DVECTOR dir = target.GetGlobalPosition() - owner.GetGlobalPosition();
          ActorInfo tgtparent = target.Relation.UseParentCoords ? target.TopParent : null;
          if (tgtparent != null)
            dir += tgtparent.GetRelativePositionXYZ(0, 0, target.GetTrueSpeed() * d) - tgtparent.GetGlobalPosition();

          /*
          ActorInfo a2 = target.AttachToParent ? engine.ActorFactory.Get(target.ParentID) : null;
          if (a2 == null)
          {
            dir = target.GetRelativePositionXYZ(0, 0, target.MoveData.Speed * d) - owner.GetPosition();
          }
          else
          {
            //dir = target.GetPosition() - owner.GetPosition();
            dir = a2.GetRelativePositionXYZ(target.GetLocalPosition().x, target.GetLocalPosition().y, target.GetLocalPosition().z + a2.MoveData.Speed * d) - owner.GetPosition();
          }
          */
          acinfo.Rotation = Utilities.GetRotation(dir);
        }
        else
        {
          acinfo.Rotation = owner.Transform.Rotation;
        }

        ActorInfo a = engine.ActorFactory.Create(acinfo);
        owner.AddChild(a);
        a.Faction = owner.Faction;

        if (a.StateModel.ComponentMask.Has(ComponentMask.HAS_AI))
          a.QueueLast(new Wait(ProjectileWaitBeforeHoming));
        a.QueueLast(new AttackActor(target, 0, 0, false, 9999));
        a.QueueLast(new Lock());

        return true;
      }
      return false;
    }
  }
}
