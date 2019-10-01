using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class SDTurboLaserATI : Groups.Turbolasers
  {
    internal SDTurboLaserATI(Factory owner) : base(owner, "IMPLLSR", "Star Destroyer Turbolaser Tower")
    {
      CombatData.MaxStrength = 16; //32
      CombatData.ImpactDamage = 16;

      ScoreData = new ScoreData(250, 1250);

      MeshData = new MeshData(Name, @"turbotowers\star_destroyer_turbolaser.x");

      Loadouts = new string[] { "IMPL_LASR" };
      DyingMoveData.Kill();
    }
  }
}

