﻿using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Actors.Data;
using SWEndor.Actors.Traits;
using SWEndor.Weapons;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class TowerGunAdvATI : Groups.SurfaceGun
  {
    internal TowerGunAdvATI(Factory owner) : base(owner, "Advanced Turbolaser Turret")
    {
      Explodes = new ExplodeInfo[] {
        new ExplodeInfo("ExpL00", 1, 3, ExplodeTrigger.ON_DEATH)
      };

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

      ainfo.WeaponSystemInfo.Weapons = new Dictionary<string, WeaponInfo>{ {"laser", WeaponFactory.Get("TOWR_LASR") }
                                                        };
      ainfo.WeaponSystemInfo.PrimaryWeapons = new string[] { "1:laser" };
      ainfo.WeaponSystemInfo.SecondaryWeapons = new string[] { "none" };
      ainfo.WeaponSystemInfo.AIWeapons = new string[] { "1:laser" };
    }
  }
}

