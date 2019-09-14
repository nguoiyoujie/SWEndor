using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class CorellianTurboLaserATI : Groups.Turbolasers
  {
    internal CorellianTurboLaserATI(Factory owner) : base(owner, "Corellian Turbolaser Tower")
    {
      MaxStrength = 105;
      ImpactDamage = 16;

      MeshData = new MeshData(Name, @"turbotowers\corellian_turbolaser.x");
      DyingMoveData.Kill();

      Loadouts = new string[] { "CRLN_LASR" };
    }
  }
}

