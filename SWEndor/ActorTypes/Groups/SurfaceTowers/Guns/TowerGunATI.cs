using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Actors.Data;
using SWEndor.Weapons;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class TowerGunATI : Groups.SurfaceGun
  {
    internal TowerGunATI(Factory owner) : base(owner, "Turbolaser Turret")
    {
      Explodes = new ExplodeInfo[] {
        new ExplodeInfo("ExpL00", 1, 3, ExplodeTrigger.ON_DEATH)
      };

      MaxStrength = 16;
      ImpactDamage = 60;
      MaxTurnRate = 36;
      ZTilt = 0;
      XLimit = 35;

      Score_perStrength = 25;
      Score_DestroyBonus = 1500;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"towers\tower_turbolaser.x");

      Loadouts = new string[] { "TOWR_DLSR" };
    }
  }
}

