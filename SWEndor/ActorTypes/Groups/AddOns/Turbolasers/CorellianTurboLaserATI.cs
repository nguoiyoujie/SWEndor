﻿using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class CorellianTurboLaserATI : Groups.Turbolasers
  {
    internal CorellianTurboLaserATI(Factory owner) : base(owner, "CORLLSR", "Corellian Turbolaser Tower")
    {
      CombatData.MaxStrength = 105;
      CombatData.ImpactDamage = 16;

      MeshData = new MeshData(Name, @"turbotowers\corellian_turbolaser.x");
      DyingMoveData.Kill();

      Loadouts = new string[] { "CRLN_LASR" };
    }
  }
}

