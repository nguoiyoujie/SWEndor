using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class NebulonBTurboLaserATI : Groups.Turbolasers
  {
    internal NebulonBTurboLaserATI(Factory owner) : base(owner, "Nebulon B Turbolaser Tower")
    {
      MaxStrength = 135;
      ImpactDamage = 16;

      MeshData = new MeshData(Name, @"turbotowers\nebulonb_turbolaser.x");

      Loadouts = new string[] { "NEBL_LASR" };
      DyingMoveData.Kill();
    }
  }
}

