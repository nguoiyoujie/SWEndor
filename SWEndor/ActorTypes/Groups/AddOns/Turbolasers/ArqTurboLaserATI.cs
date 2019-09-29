using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class ArqTurboLaserATI : Groups.Turbolasers
  {
    internal ArqTurboLaserATI(Factory owner) : base(owner, "Arquitens Turbolaser Tower")
    {
      MaxStrength = 80;
      ImpactDamage = 16;

      MeshData = new MeshData(Name, @"turbotowers\acclamator_turbolaser.x");
      DyingMoveData.Kill();

      Loadouts = new string[] { "ARQT_LASR" };
    }
  }
}

