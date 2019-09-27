using SWEndor.Actors;
using SWEndor.Actors.Models;
using SWEndor.ActorTypes.Components;
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
      MoveLimitData.MaxTurnRate = 60;
      MoveLimitData.ZTilt = 0;
      MoveLimitData.XLimit = 55;

      ScoreData = new ScoreData(100, 3500);

      MeshData = new MeshData(Name, @"towers\tower_turbolaser.x");

      Loadouts = new string[] { "TOWR_SLSR" };
    }
  }
}

