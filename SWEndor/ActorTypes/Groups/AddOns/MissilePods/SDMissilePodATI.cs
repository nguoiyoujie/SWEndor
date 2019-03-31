using SWEndor.Weapons;
using SWEndor.Weapons.Types;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.Actors.Types
{
  public class SDMissilePodATI : AddOnGroup
  {
    private static SDMissilePodATI _instance;
    public static SDMissilePodATI Instance()
    {
      if (_instance == null) { _instance = new SDMissilePodATI(); }
      return _instance;
    }

    private SDMissilePodATI() : base("Star Destroyer Missile Pod")
    {
      // Combat
      IsCombatObject = true;
      IsSelectable = true;
      IsDamage = false;
      CollisionEnabled = true;

      MaxStrength = 24; //32
      ImpactDamage = 16;

      Score_perStrength = 250;
      Score_DestroyBonus = 1250;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"turbotowers\star_destroyer_missilepod.x");
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.ExplosionInfo.EnableDeathExplosion = true;
      ainfo.ExplosionInfo.DeathExplosionType = "Explosion";
      ainfo.ExplosionInfo.DeathExplosionSize = 5;

      ainfo.Weapons = new Dictionary<string, WeaponInfo>{ {"missile", new ImperialAntiShipMissileWeapon() }
                                                        };
      ainfo.PrimaryWeapons = new string[] { "1:missile" };
      ainfo.SecondaryWeapons = new string[] { "none" };
      ainfo.AIWeapons = new string[] { "1:missile" };
    }
  }
}

