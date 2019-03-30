using SWEndor.Weapons;
using SWEndor.Weapons.Types;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.Actors.Types
{
  public class TowerGunSuperATI : SurfaceTowerGroup
  {
    private static TowerGunSuperATI _instance;
    public static TowerGunSuperATI Instance()
    {
      if (_instance == null) { _instance = new TowerGunSuperATI(); }
      return _instance;
    }

    private TowerGunSuperATI() : base("Super Turbolaser Turret")
    {
      MaxStrength = 45;
      ImpactDamage = 60;
      MaxTurnRate = 60;
      ZTilt = 0;
      XLimit = 55;

      RadarSize = 0;

      Score_perStrength = 100;
      Score_DestroyBonus = 3500;

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

      ainfo.Weapons = new Dictionary<string, WeaponInfo>{ {"laser", new Tower_SuperLaserWeapon() }
                                                        };
      ainfo.PrimaryWeapons = new string[] { "1:laser" };
      ainfo.SecondaryWeapons = new string[] { "none" };
      ainfo.AIWeapons = new string[] { "1:laser" };
    }
  }
}

