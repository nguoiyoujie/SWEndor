using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class TowerGunAdvATI : SurfaceTowerGroup
  {
    private static TowerGunAdvATI _instance;
    public static TowerGunAdvATI Instance()
    {
      if (_instance == null) { _instance = new TowerGunAdvATI(); }
      return _instance;
    }

    private TowerGunAdvATI() : base("Advanced Turbolaser Turret")
    {
      // Combat
      IsCombatObject = true;
      IsSelectable = true;
      IsDamage = false;
      CollisionEnabled = true;

      MaxStrength = 45;
      ImpactDamage = 60;
      MaxTurnRate = 48;
      ZTilt = 0;
      XLimit = 55;

      Score_perStrength = 100;
      Score_DestroyBonus = 3500;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"towers\tower_turbolaser.x");
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      // Tower Gun
      ainfo.EnableDeathExplosion = true;
      ainfo.DeathExplosionType = "ExplosionSm";
      ainfo.DeathExplosionSize = 3;

      ainfo.Weapons = new Dictionary<string, WeaponInfo>{ {"laser", new Tower_LaserWeapon() }
                                                        };
      ainfo.PrimaryWeapons = new List<string> { "1:laser" };
      ainfo.SecondaryWeapons = new List<string> { "none" };
      ainfo.AIWeapons = new List<string> { "1:laser" };
    }
  }
}

