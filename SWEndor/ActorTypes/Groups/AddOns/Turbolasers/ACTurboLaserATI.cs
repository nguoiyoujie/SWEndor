using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class ACTurboLaserATI : Groups.Turbolasers
  {
    internal ACTurboLaserATI(Factory owner) : base(owner, "Acclamator Turbolaser Tower")
    {
      MaxStrength = 85;
      ImpactDamage = 16;

      MeshData = new MeshData(Name, @"turbotowers\acclamator_turbolaser.x");
      DyingMoveData.Kill();

      Loadouts = new string[] { "ACCL_LASR" };
    }
  }
}

