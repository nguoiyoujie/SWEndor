using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class mc90TurbolaserATI : Groups.Turbolasers
  {
    internal mc90TurbolaserATI(Factory owner) : base(owner, "MC90 Turbolaser Tower")
    {
      MaxStrength = 200;
      ImpactDamage = 16;
      MoveLimitData.MaxTurnRate = 50f;

      MeshData = new MeshData(Name, @"turbotowers\mc90_turbolaser.x");
      DyingMoveData.Kill();

      Loadouts = new string[] { "MC90_LASR" };
    }
  }
}

