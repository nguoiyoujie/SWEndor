﻿using SWEndor.ActorTypes.Components;

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

      MeshData = new MeshData(Name, @"turbotowers\executor_turbolaser.x");

      Loadouts = new string[] { "EXEC_LASR" };
    }
  }
}
