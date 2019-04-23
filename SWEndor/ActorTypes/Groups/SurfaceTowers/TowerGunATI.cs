﻿using SWEndor.Actors;
using SWEndor.Weapons;
using SWEndor.Weapons.Types;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class TowerGunATI : Groups.SurfaceTower
  {
    private static TowerGunATI _instance;
    public static TowerGunATI Instance()
    {
      if (_instance == null) { _instance = new TowerGunATI(); }
      return _instance;
    }

    private TowerGunATI() : base("Turbolaser Turret")
    {
      MaxStrength = 16;
      ImpactDamage = 60;
      MaxTurnRate = 36;
      ZTilt = 0;
      XLimit = 35;

      RadarSize = 0;

      Score_perStrength = 25;
      Score_DestroyBonus = 1500;

      TargetType = TargetType.ADDON;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"towers\tower_turbolaser.x");
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      // Tower Gun
      ainfo.ExplosionInfo.EnableDeathExplosion = true;
      ainfo.ExplosionInfo.DeathExplosionType = "ExplosionSm";
      ainfo.ExplosionInfo.DeathExplosionSize = 3;

      ainfo.WeaponSystemInfo.Weapons = new Dictionary<string, WeaponInfo>{ {"laser", new Tower_DblLaserWeapon() }
                                                        };
      ainfo.WeaponSystemInfo.PrimaryWeapons = new string[] { "1:laser" };
      ainfo.WeaponSystemInfo.SecondaryWeapons = new string[] { "none" };
      ainfo.WeaponSystemInfo.AIWeapons = new string[] { "1:laser" };
    }
  }
}

