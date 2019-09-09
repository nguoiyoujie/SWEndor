using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class ExecutorTurboLaserATI : Groups.Turbolasers
  {
    internal ExecutorTurboLaserATI(Factory owner) : base(owner, "Executor Super Star Destroyer Turbolaser Tower")
    {
      MaxStrength = 105;
      ImpactDamage = 16;

      ScoreData = new ScoreData(300, 2500);

      MeshData = new MeshData(Name, @"turbotowers\executor_turbolaser.x");

      Loadouts = new string[] { "EXEC_LASR" };
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.DyingMoveComponent = DyingKill.Instance;
    }
  }
}

