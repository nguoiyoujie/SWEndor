using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class SDAntiShipTurboLaserATI : Groups.Turbolasers
  {
    internal SDAntiShipTurboLaserATI(Factory owner) : base(owner, "Star Destroyer Heavy Turbolaser Tower")
    {
      MaxStrength = 12; //25
      ImpactDamage = 16;

      ScoreData = new ScoreData(250, 1250);

      MeshData = new MeshData(Name, @"turbotowers\star_destroyer_anti-ship_turbolaser.x");
      DyingMoveData.Kill();

      Loadouts = new string[] { "IMPL_3LSR" };
    }
  }
}

