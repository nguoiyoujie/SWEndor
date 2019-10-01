using SWEndor.ActorTypes.Components;
using SWEndor.Models;

namespace SWEndor.ActorTypes.Instances
{
  public class TowerGunATI : Groups.SurfaceGun
  {
    internal TowerGunATI(Factory owner) : base(owner, "TGUN", "Turbolaser Turret")
    {
      Explodes = new ExplodeData[] {
        new ExplodeData("EXPL00", 1, 3, ExplodeTrigger.ON_DEATH)
      };

      MaxStrength = 16;
      ImpactDamage = 60;
      MoveLimitData.MaxTurnRate = 36;
      MoveLimitData.ZTilt = 0;
      MoveLimitData.XLimit = 35;

      ScoreData = new ScoreData(25, 1500);

      MeshData = new MeshData(Name, @"towers\tower_turbolaser.x");

      Loadouts = new string[] { "TOWR_DLSR" };
    }
  }
}

