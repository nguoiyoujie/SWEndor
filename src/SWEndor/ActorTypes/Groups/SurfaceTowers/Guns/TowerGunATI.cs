using SWEndor.ActorTypes.Components;
using SWEndor.Models;

namespace SWEndor.ActorTypes.Instances
{
  internal class TowerGunATI : Groups.SurfaceGun
  {
    internal TowerGunATI(Factory owner) : base(owner, "TGUN", "Turbolaser Turret")
    {
      ExplodeSystemData.Explodes = new ExplodeData[] {
        new ExplodeData("EXPL00", 1, 3, ExplodeTrigger.ON_DEATH)
      };

      SystemData.MaxShield = 8;
      SystemData.MaxHull = 8;
      CombatData.ImpactDamage = 60;
      MoveLimitData.MaxTurnRate = 36;
      MoveLimitData.ZTilt = 0;
      MoveLimitData.XLimit = 35;

      ScoreData = new ScoreData(25, 1500);

      MeshData = new MeshData(Engine, Name, @"towers\tower_turbolaser.x");

      WeapSystemData.Loadouts = new WeapData[] { new WeapData("LASR", "PRI_1_AI", "NO_AUTOAIM", "DEFAULT", "TOWR_LASR", "WING_LSR_Y2", "WING_LASER") };
    }
  }
}

