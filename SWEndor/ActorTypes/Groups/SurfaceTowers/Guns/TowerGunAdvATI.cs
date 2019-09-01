using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Actors.Data;
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

      Loadouts = new string[] { "TOWR_LASR" };
    }
  }
}

