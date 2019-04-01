﻿using SWEndor.Actors;
using SWEndor.Weapons;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.ActorTypes
{
  public class ArqTurboLaserATI : AddOnGroup
  {
    private static ArqTurboLaserATI _instance;
    public static ArqTurboLaserATI Instance()
    {
      if (_instance == null) { _instance = new ArqTurboLaserATI(); }
      return _instance;
    }

    private ArqTurboLaserATI() : base("Arquitens Turbolaser Tower")
    {
      // Combat
      IsCombatObject = true;
      IsSelectable = true;
      IsDamage = false;
      CollisionEnabled = true;

      MaxStrength = 15;
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

      ainfo.Weapons = new Dictionary<string, WeaponInfo>{ {"laser", WeaponFactory.Get("Arquitens Laser") } //new AcclamatorTurboLaserWeapon() }
                                                        };
      ainfo.PrimaryWeapons = new string[] { "1:laser" };
      ainfo.SecondaryWeapons = new string[] { "none" };
      ainfo.AIWeapons = new string[] { "1:laser" };
    }
  }
}

