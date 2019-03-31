﻿using SWEndor.Weapons;
using SWEndor.Weapons.Types;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.Actors.Types
{
  public class NebulonBMissilePodATI : AddOnGroup
  {
    private static NebulonBMissilePodATI _instance;
    public static NebulonBMissilePodATI Instance()
    {
      if (_instance == null) { _instance = new NebulonBMissilePodATI(); }
      return _instance;
    }

    private NebulonBMissilePodATI() : base("Nebulon B Missile Pod")
    {
      // Combat
      IsCombatObject = true;
      IsSelectable = true;
      IsDamage = false;
      CollisionEnabled = true;

      MaxStrength = 140; //32
      ImpactDamage = 16;

      Score_perStrength = 250;
      Score_DestroyBonus = 1250;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"turbotowers\nebulonb_missilepod.x");
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.ExplosionInfo.EnableDeathExplosion = true;
      ainfo.ExplosionInfo.DeathExplosionType = "Explosion";
      ainfo.ExplosionInfo.DeathExplosionSize = 5;

      ainfo.Weapons = new Dictionary<string, WeaponInfo>{ {"missile", new NebulonBMissileWeapon() }
                                                        };
      ainfo.PrimaryWeapons = new string[] { "1:missile" };
      ainfo.SecondaryWeapons = new string[] { "none" };
      ainfo.AIWeapons = new string[] { "1:missile" };
    }
  }
}

