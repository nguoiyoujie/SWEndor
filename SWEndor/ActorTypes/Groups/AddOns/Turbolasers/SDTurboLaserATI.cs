using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class SDTurboLaserATI : Groups.Turbolasers
  {
    internal SDTurboLaserATI(Factory owner) : base(owner, "Star Destroyer Turbolaser Tower")
    {
      MaxStrength = 16; //32
      ImpactDamage = 16;

      ScoreData = new ScoreData(250, 1250);

      MeshData = new MeshData(Name, @"turbotowers\star_destroyer_turbolaser.x");

      Loadouts = new string[] { "IMPL_LASR" };
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.DyingMoveComponent = DyingKill.Instance;
    }
  }
}

