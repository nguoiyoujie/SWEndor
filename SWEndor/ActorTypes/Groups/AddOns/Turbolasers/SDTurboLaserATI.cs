﻿using SWEndor.Weapons;
using SWEndor.Weapons.Types;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.Actors.Types
{
  public class SDTurboLaserATI : AddOnGroup
  {
    private static SDTurboLaserATI _instance;
    public static SDTurboLaserATI Instance()
    {
      if (_instance == null) { _instance = new SDTurboLaserATI(); }
      return _instance;
    }

    private SDTurboLaserATI() : base("Star Destroyer Turbolaser Tower")
    {
      // Combat
      IsCombatObject = true;
      IsSelectable = true;
      IsDamage = false;
      CollisionEnabled = true;

      MaxStrength = 16; //32
      ImpactDamage = 16;

      Score_perStrength = 250;
      Score_DestroyBonus = 1250;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"turbotowers\star_destroyer_turbolaser.x");
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.ExplosionInfo.EnableDeathExplosion = true;
      ainfo.ExplosionInfo.DeathExplosionType = "Explosion";
      ainfo.ExplosionInfo.DeathExplosionSize = 5;

      ainfo.Weapons = new Dictionary<string, WeaponInfo>{ {"laser", new ImperialTurboLaserWeapon() }
                                                        };
      ainfo.PrimaryWeapons = new string[] { "1:laser" };
      ainfo.SecondaryWeapons = new string[] { "none" };
      ainfo.AIWeapons = new string[] { "1:laser" };
    }
  }
}

