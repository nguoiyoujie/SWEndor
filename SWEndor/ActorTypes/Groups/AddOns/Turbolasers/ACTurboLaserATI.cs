using SWEndor.Actors;
using SWEndor.Weapons;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.ActorTypes
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

      ainfo.ExplosionInfo.EnableDeathExplosion = true;
      ainfo.ExplosionInfo.DeathExplosionType = "Explosion";
      ainfo.ExplosionInfo.DeathExplosionSize = 5;

      ainfo.WeaponSystemInfo.Weapons = new Dictionary<string, WeaponInfo>{ {"laser", WeaponFactory.Get("Acclamator Laser") } // new AcclamatorTurboLaserWeapon() }
                                                        };
      ainfo.WeaponSystemInfo.PrimaryWeapons = new string[] { "1:laser" };
      ainfo.WeaponSystemInfo.SecondaryWeapons = new string[] { "none" };
      ainfo.WeaponSystemInfo.AIWeapons = new string[] { "1:laser" };
    }
  }
}

