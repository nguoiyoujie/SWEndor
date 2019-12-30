using SWEndor.ActorTypes.Components;
using SWEndor.Models;

namespace SWEndor.ActorTypes.Instances
{
  internal class JV7TurboLaserATI : Groups.Turbolasers
  {
    internal JV7TurboLaserATI(Factory owner) : base(owner, "JV7LSR", "Laser Turret")
    {
      SystemData.MaxShield = 4;
      SystemData.MaxHull = 8;
      CombatData.ImpactDamage = 16;
      RenderData.RadarSize = 0;
      AIData.TargetType = TargetType.NULL;

      MeshData = new MeshData(Engine, Name, @"turbotowers\sm_turbolaser.x");
      DyingMoveData.Kill();

      WeapSystemData.Loadouts = new WeapData[] { new WeapData("", "AI", "ADDON_TURBOLASER", "JV7_LASR", "JV7_LASR", "ADDON_LSR_G", "ADDON_TURBOLASER") };
    }
  }
}

