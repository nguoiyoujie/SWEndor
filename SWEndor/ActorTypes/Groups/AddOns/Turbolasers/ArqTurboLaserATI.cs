using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class ArqTurboLaserATI : Groups.Turbolasers
  {
    internal ArqTurboLaserATI(Factory owner) : base(owner, "Arquitens Turbolaser Tower")
    {
      MaxStrength = 80;
      ImpactDamage = 16;

      MeshData = new MeshData(Name, @"turbotowers\acclamator_turbolaser.x");

      Loadouts = new string[] { "ARQT_LASR" };
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.DyingMoveComponent = DyingKill.Instance;
    }
  }
}

