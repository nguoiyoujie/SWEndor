using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class SDAntiShipTurboLaserATI : Groups.Turbolasers
  {
    internal SDAntiShipTurboLaserATI(Factory owner) : base(owner, "IMPLHLSR", "Star Destroyer Heavy Turbolaser Tower")
    {
      SystemData.MaxShield = 8;
      SystemData.MaxHull = 4;
      CombatData.ImpactDamage = 16;

      ScoreData = new ScoreData(250, 1250);

      MeshData = new MeshData(Name, @"turbotowers\star_destroyer_anti-ship_turbolaser.x");
      DyingMoveData.Kill();

      Loadouts = new string[] { "IMPL_3LSR" };
    }
  }
}

