﻿using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class T4ATurboLaserATI : Groups.Turbolasers
  {
    internal T4ATurboLaserATI(Factory owner) : base(owner, "T4ALSR", "Laser Turret")
    {
      CombatData.MaxStrength = 6;
      CombatData.ImpactDamage = 16;
      RenderData.RadarSize = 0;
      AIData.TargetType = TargetType.NULL;

      MeshData = new MeshData(Name, @"turbotowers\xq_turbolaser.x", 0.25f);
      DyingMoveData.Kill();

      Loadouts = new string[] { "T4A_LASR" };
    }
  }
}

