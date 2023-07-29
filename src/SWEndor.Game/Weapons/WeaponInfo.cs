using SWEndor.Game.Actors;
using SWEndor.Game.Actors.Models;
using SWEndor.Game.Core;
using SWEndor.Game.Models;

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

    private bool CreateProjectile(Engine engine, ActorInfo owner, ActorInfo target)
    {
      if (owner == null)
        return false;

      if (Proj.WeaponType == WeaponType.LASER && !owner.IsSystemOperational(SystemPart.LASER_WEAPONS))
        return false;
      else if (Proj.WeaponType == WeaponType.ION && !owner.IsSystemOperational(SystemPart.ION_WEAPONS))
        return false;
      else if ((Proj.WeaponType == WeaponType.MISSILE || Proj.WeaponType == WeaponType.TORPEDO) && !owner.IsSystemOperational(SystemPart.PROJECTILE_LAUNCHERS))
        return false;


      if (owner.IsPlayer
        && !engine.PlayerInfo.PlayerAIEnabled
        && (!Targeter.RequirePlayerTargetLock || target != null))
      { // Player
        return Proj.Create(engine, owner, target, true, Port.GetFirePos(), ref Aim);
      }

      if ((target == null && Targeter.AIAttackNull)
          || (target != null && target.TargetType.Intersects(Targeter.AIAttackTargets)
          ))
      { // AI 
        return Proj.Create(engine, owner, target, false, Port.GetFirePos(), ref Aim);
      }
      return false;
    }
  }
}
