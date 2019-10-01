using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class ExecutorTurboLaserATI : Groups.Turbolasers
  {
    internal ExecutorTurboLaserATI(Factory owner) : base(owner, "EXECLSR", "Executor Super Star Destroyer Turbolaser Tower")
    {
      MaxStrength = 105;
      ImpactDamage = 16;

      ScoreData = new ScoreData(300, 2500);
      DyingMoveData.Kill();

      MeshData = new MeshData(Name, @"turbotowers\executor_turbolaser.x");

      Loadouts = new string[] { "EXEC_LASR" };
    }
  }
}

