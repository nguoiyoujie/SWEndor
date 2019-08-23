using SWEndor.Actors;
using SWEndor.Actors.Data;
using SWEndor.AI.Actions;
using SWEndor.Weapons;
using SWEndor.Weapons.Types;
using System.Collections.Generic;

namespace SWEndor.ActorTypes.Groups
{
  public class MissileProjectile : Projectile
  {
    internal MissileProjectile(Factory owner, string name) : base(owner, name)
    {
      // Combat
      ExplodeData = new ExplodeData(deathTrigger: DeathExplosionTrigger.TIMENOTEXPIRED_ONLY, deathExplosionType: "ExplosionSm");
      Armor = ActorInfo.ArmorModel.Default;
      CombatData = CombatData.DefaultFighter;

      CullDistance = 12000;

      // Projectile
      ImpactCloseEnoughDistance = 100;
      TargetType = TargetType.MUNITION;
      RadarType = RadarType.TRAILLINE;

      Mask = ComponentMask.GUIDED_PROJECTILE;
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.WeaponSystemInfo.Weapons = new Dictionary<string, WeaponInfo>{ {"dummy", new TrackerDummyWeapon() }
                                                        };
      ainfo.WeaponSystemInfo.PrimaryWeapons = new string[] { "1:dummy" };
      ainfo.WeaponSystemInfo.SecondaryWeapons = new string[] { "none" };
      ainfo.WeaponSystemInfo.AIWeapons = new string[] { "1:dummy" };
    }

    public override void ProcessState(ActorInfo ainfo)
    {
      base.ProcessState(ainfo);
      if (ainfo.CurrentAction == null || ainfo.CurrentAction is Idle)
        ainfo.SetState_Dead();
    }
  }
}


