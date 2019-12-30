﻿using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  internal class TransportTurboLaserATI : Groups.Turbolasers
  {
    internal TransportTurboLaserATI(Factory owner) : base(owner, "TRANLSR", "Transport Turbolaser Tower")
    {
      SystemData.MaxShield = 80;
      CombatData.ImpactDamage = 16;

      MeshData = new MeshData(Engine, Name, @"turbotowers\transport_turbolaser.x");
      DyingMoveData.Kill();

      WeapSystemData.Loadouts = new WeapData[] { new WeapData("", "AI", "ADDON_TURBOLASER", "TRNS_LASR", "TRNS_LASR", "ADDON_LSR_R", "ADDON_TURBOLASER") };
    }
  }
}

