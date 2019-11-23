﻿using SWEndor.ActorTypes.Components;
using SWEndor.Models;

namespace SWEndor.ActorTypes.Instances
{
  internal class T4ATurboLaserATI : Groups.Turbolasers
  {
    internal T4ATurboLaserATI(Factory owner) : base(owner, "T4ALSR", "Laser Turret")
    {
      SystemData.MaxShield = 4;
      SystemData.MaxHull = 3;
      CombatData.ImpactDamage = 16;
      RenderData.RadarSize = 0;
      AIData.TargetType = TargetType.NULL;

      MeshData = new MeshData(Engine, Name, @"turbotowers\xq_turbolaser.x", 0.25f);
      DyingMoveData.Kill();

      Loadouts = new WeapData[] { new WeapData("", "AI", "ADDON_TURBOLASER", "T4A_LASR", "T4A_LASR", "ADDON_LSR_G", "ADDON_TURBOLASER") };
    }
  }
}

