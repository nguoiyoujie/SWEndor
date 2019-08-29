using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Actors.Data;
using SWEndor.Weapons;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class TowerGunSuperATI : Groups.SurfaceGun
  {
    internal TowerGunSuperATI(Factory owner) : base(owner, "Super Turbolaser Turret")
    {
      ExplodeData = new ExplodeData(deathTrigger: DeathExplosionTrigger.ALWAYS, deathExplosionSize: 3);

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

