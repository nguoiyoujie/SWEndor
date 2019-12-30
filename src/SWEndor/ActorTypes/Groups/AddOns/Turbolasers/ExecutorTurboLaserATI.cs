using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  internal class ExecutorTurboLaserATI : Groups.Turbolasers
  {
    internal ExecutorTurboLaserATI(Factory owner) : base(owner, "EXECLSR", "Executor Super Star Destroyer Turbolaser Tower")
    {
      SystemData.MaxShield = 30;
      SystemData.MaxHull = 45;
      CombatData.ImpactDamage = 16;

      ScoreData = new ScoreData(300, 2500);
      DyingMoveData.Kill();

      MeshData = new MeshData(Engine, Name, @"turbotowers\executor_turbolaser.x");

      WeapSystemData.Loadouts = new WeapData[] { new WeapData("", "AI", "ADDON_TURBOLASER", "EXEC_LASR", "EXEC_LASR", "ADDON_LSR_Y", "ADDON_TURBOLASER") };
    }
  }
}

