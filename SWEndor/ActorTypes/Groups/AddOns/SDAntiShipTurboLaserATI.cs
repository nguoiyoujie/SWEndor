using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class SDAntiShipTurboLaserATI : AddOnGroup
  {
    private static SDAntiShipTurboLaserATI _instance;
    public static SDAntiShipTurboLaserATI Instance()
    {
      if (_instance == null) { _instance = new SDAntiShipTurboLaserATI(); }
      return _instance;
    }

    private SDAntiShipTurboLaserATI() : base("Star Destroyer Heavy Turbolaser Tower")
    {
      // Combat
      IsCombatObject = true;
      IsSelectable = true;
      IsDamage = false;
      CollisionEnabled = true;

      MaxStrength = 25;
      ImpactDamage = 16;

      Score_perStrength = 250;
      Score_DestroyBonus = 1250;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"turbotowers\star_destroyer_anti-ship_turbolaser.x");
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.EnableDeathExplosion = true;
      ainfo.DeathExplosionType = "Explosion";
      ainfo.DeathExplosionSize = 5;

      ainfo.Weapons = new Dictionary<string, WeaponInfo>{ {"aship", new ImperialAntiShipTurboLaserWeapon() }
                                                        };
      ainfo.PrimaryWeapons = new List<string> { "1:aship" };
      ainfo.SecondaryWeapons = new List<string> { "none" };
      ainfo.AIWeapons = new List<string> { "1:aship" };
    }
  }
}

