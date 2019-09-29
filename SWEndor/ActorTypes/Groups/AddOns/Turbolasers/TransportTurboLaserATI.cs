using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class TransportTurboLaserATI : Groups.Turbolasers
  {
    internal TransportTurboLaserATI(Factory owner) : base(owner, "Transport Turbolaser Tower")
    {
      MaxStrength = 80;
      ImpactDamage = 16;

      MeshData = new MeshData(Name, @"turbotowers\transport_turbolaser.x");
      DyingMoveData.Kill();

      Loadouts = new string[] { "TRNS_LASR" };
    }
  }
}

