using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Actors.Data;
using SWEndor.Weapons;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class TowerGunSuperATI : Groups.SurfaceTower
  {
    internal TowerGunSuperATI(Factory owner) : base(owner, "Super Turbolaser Turret")
    {
      ExplodeData = new ExplodeData(deathTrigger: DeathExplosionTrigger.ALWAYS, deathExplosionSize: 3);

      MaxStrength = 45;
      ImpactDamage = 60;
      MaxTurnRate = 60;
      ZTilt = 0;
      XLimit = 55;

      RadarSize = 0;

      Score_perStrength = 100;
      Score_DestroyBonus = 3500;

      TargetType = TargetType.ADDON;
      RadarType = RadarType.NULL;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"towers\tower_turbolaser.x");
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.WeaponSystemInfo.Weapons = new Dictionary<string, WeaponInfo>{ {"laser", WeaponFactory.Get("TOWR_SLSR") }
                                                        };
      ainfo.WeaponSystemInfo.PrimaryWeapons = new string[] { "1:laser" };
      ainfo.WeaponSystemInfo.SecondaryWeapons = new string[] { "none" };
      ainfo.WeaponSystemInfo.AIWeapons = new string[] { "1:laser" };
    }
  }
}

