using SWEndor.ActorTypes.Components;
using SWEndor.Models;

namespace SWEndor.ActorTypes.Instances
{
  public class TowerGunSuperATI : Groups.SurfaceGun
  {
    internal TowerGunSuperATI(Factory owner) : base(owner, "SUPGUN", "Super Turbolaser Turret")
    {
      Explodes = new ExplodeData[] {
        new ExplodeData("EXPL00", 1, 3, ExplodeTrigger.ON_DEATH)
      };

      SystemData.MaxShield = 20;
      SystemData.MaxHull = 25;
      CombatData.ImpactDamage = 60;
      MoveLimitData.MaxTurnRate = 60;
      MoveLimitData.ZTilt = 0;
      MoveLimitData.XLimit = 55;

      ScoreData = new ScoreData(100, 3500);

      MeshData = new MeshData(Name, @"towers\tower_turbolaser.x");

      Loadouts = new string[] { "TOWR_SLSR" };
    }
  }
}

