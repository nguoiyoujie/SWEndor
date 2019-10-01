using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class ArqTurboLaserATI : Groups.Turbolasers
  {
    internal ArqTurboLaserATI(Factory owner) : base(owner, "ARQTLSR", "Arquitens Turbolaser Tower")
    {
      CombatData.MaxStrength = 80;
      CombatData.ImpactDamage = 16;

      MeshData = new MeshData(Name, @"turbotowers\acclamator_turbolaser.x");
      DyingMoveData.Kill();

      Loadouts = new string[] { "ARQT_LASR" };
    }
  }
}

