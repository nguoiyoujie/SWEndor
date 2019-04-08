using SWEndor.Actors;
using SWEndor.Weapons;
using SWEndor.Weapons.Types;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.ActorTypes
{
  public class CorellianTurboLaserATI : AddOnGroup
  {
    private static CorellianTurboLaserATI _instance;
    public static CorellianTurboLaserATI Instance()
    {
      if (_instance == null) { _instance = new CorellianTurboLaserATI(); }
      return _instance;
    }

    private CorellianTurboLaserATI() : base("Corellian Turbolaser Tower")
    {
      // Combat
      IsCombatObject = true;
      IsSelectable = true;
      IsDamage = false;
      CollisionEnabled = true;

      MaxStrength = 105;
      ImpactDamage = 16;

      Score_perStrength = 250;
      Score_DestroyBonus = 1250;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"turbotowers\corellian_turbolaser.x");
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.ExplosionInfo.EnableDeathExplosion = true;
      ainfo.ExplosionInfo.DeathExplosionType = "Explosion";
      ainfo.ExplosionInfo.DeathExplosionSize = 5;

      ainfo.WeaponSystemInfo.Weapons = new Dictionary<string, WeaponInfo>{ {"laser", new CorellianTurboLaserWeapon() }
                                                        };
      ainfo.WeaponSystemInfo.PrimaryWeapons = new string[] { "1:laser" };
      ainfo.WeaponSystemInfo.SecondaryWeapons = new string[] { "none" };
      ainfo.WeaponSystemInfo.AIWeapons = new string[] { "1:laser" };
    }
  }
}

