﻿using SWEndor.Actors;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class TowerGunSuperATI : Groups.SurfaceGun
  {
    internal TowerGunSuperATI(Factory owner) : base(owner, "Super Turbolaser Turret")
    {
      Explodes = new ExplodeInfo[] {
        new ExplodeInfo("ExpL00", 1, 3, ExplodeTrigger.ON_DEATH)
      };

      MaxStrength = 45;
      ImpactDamage = 60;
      MaxTurnRate = 60;
      ZTilt = 0;
      XLimit = 55;

      Score_perStrength = 100;
      Score_DestroyBonus = 3500;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"towers\tower_turbolaser.x");

      Loadouts = new string[] { "TOWR_SLSR" };
    }
  }
}

