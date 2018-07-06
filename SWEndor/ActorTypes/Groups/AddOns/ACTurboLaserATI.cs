using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class ACTurboLaserATI : AddOnGroup
  {
    private static ACTurboLaserATI _instance;
    public static ACTurboLaserATI Instance()
    {
      if (_instance == null) { _instance = new ACTurboLaserATI(); }
      return _instance;
    }

    private ACTurboLaserATI() : base("Acclamator Turbolaser Tower")
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

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"turbotowers\acclamator_turbolaser.x");
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.EnableDeathExplosion = true;
      ainfo.DeathExplosionType= "Explosion";
      ainfo.DeathExplosionSize = 5;

      ainfo.Weapons = new Dictionary<string, WeaponInfo>{ {"laser", new AcclamatorTurboLaserWeapon() }
                                                        };
      ainfo.PrimaryWeapons = new List<string> { "1:laser" };
      ainfo.SecondaryWeapons = new List<string> { "none" };
      ainfo.AIWeapons = new List<string> { "1:laser" };
    }
  }
}

