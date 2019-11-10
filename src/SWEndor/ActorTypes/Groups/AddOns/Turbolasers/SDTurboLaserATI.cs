using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  internal class SDTurboLaserATI : Groups.Turbolasers
  {
    internal SDTurboLaserATI(Factory owner) : base(owner, "IMPLLSR", "Star Destroyer Turbolaser Tower")
    {
      SystemData.MaxShield = 10;
      SystemData.MaxHull = 6;
      CombatData.ImpactDamage = 16;

      ScoreData = new ScoreData(250, 1250);

      MeshData = new MeshData(Name, @"turbotowers\star_destroyer_turbolaser.x");

      Loadouts = new string[] { "IMPL_LASR" };
      DyingMoveData.Kill();
    }
  }
}

