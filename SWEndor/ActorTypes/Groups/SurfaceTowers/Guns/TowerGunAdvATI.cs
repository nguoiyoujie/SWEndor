using SWEndor.ActorTypes.Components;
using SWEndor.Models;

namespace SWEndor.ActorTypes.Instances
{
  public class TowerGunAdvATI : Groups.SurfaceGun
  {
    internal TowerGunAdvATI(Factory owner) : base(owner, "ADVGUN", "Advanced Turbolaser Turret")
    {
      Explodes = new ExplodeData[] {
        new ExplodeData("EXPL00", 1, 3, ExplodeTrigger.ON_DEATH)
      };

      CombatData.MaxStrength = 45;
      CombatData.ImpactDamage = 60;
      MoveLimitData.MaxTurnRate = 48;
      MoveLimitData.ZTilt = 0;
      MoveLimitData.XLimit = 55;

      ScoreData = new ScoreData(100, 3500);

      MeshData = new MeshData(Name, @"towers\tower_turbolaser.x");

      Loadouts = new string[] { "TOWR_LASR" };
    }
  }
}

