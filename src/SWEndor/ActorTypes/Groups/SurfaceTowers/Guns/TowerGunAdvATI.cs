using SWEndor.ActorTypes.Components;
using SWEndor.Models;

namespace SWEndor.ActorTypes.Instances
{
  internal class TowerGunAdvATI : Groups.SurfaceGun
  {
    internal TowerGunAdvATI(Factory owner) : base(owner, "ADVGUN", "Advanced Turbolaser Turret")
    {
      Explodes = new ExplodeData[] {
        new ExplodeData("EXPL00", 1, 3, ExplodeTrigger.ON_DEATH)
      };

      SystemData.MaxShield = 20;
      SystemData.MaxHull = 25;
      CombatData.ImpactDamage = 60;
      MoveLimitData.MaxTurnRate = 48;
      MoveLimitData.ZTilt = 0;
      MoveLimitData.XLimit = 55;

      ScoreData = new ScoreData(100, 3500);

      MeshData = new MeshData(Engine, Name, @"towers\tower_turbolaser.x");

      Loadouts = new WeapData[] { new WeapData("LASR", "PRI_1_AI", "NO_AUTOAIM", "DEFAULT", "TOWR_DLSR", "WING_LSR_G2", "WING_LASER") };
    }
  }
}

