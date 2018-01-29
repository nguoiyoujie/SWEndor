using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class TowerGunATI : SurfaceTowerGroup
  {
    private static TowerGunATI _instance;
    public static TowerGunATI Instance()
    {
      if (_instance == null) { _instance = new TowerGunATI(); }
      return _instance;
    }

    private TowerGunATI() : base("Turbolaser Turret")
    {
      // Combat
      IsCombatObject = true;
      IsSelectable = true;
      IsDamage = false;
      CollisionEnabled = true;

      MaxStrength = 16;
      ImpactDamage = 60;
      MaxTurnRate = 36;
      ZTilt = 0;
      XLimit = 35;

      RadarSize = 1;

      Score_perStrength = 25;
      Score_DestroyBonus = 1500;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"towers\tower_turbolaser.x");
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      // Tower Gun
      ainfo.EnableDeathExplosion = true;
      ainfo.DeathExplosionType = "ExplosionSm";
      ainfo.DeathExplosionSize = 3;

      ainfo.Weapons = new Dictionary<string, WeaponInfo>{ {"laser", new Tower_DblLaserWeapon() }
                                                        };
      ainfo.PrimaryWeapons = new List<string> { "1:laser" };
      ainfo.SecondaryWeapons = new List<string> { "none" };
      ainfo.AIWeapons = new List<string> { "1:laser" };
    }
  }
}

