using SWEndor.ActorTypes.Components;
using SWEndor.Models;

namespace SWEndor.ActorTypes.Instances
{
  internal class CargoTurboLaserATI : Groups.Turbolasers
  {
    internal CargoTurboLaserATI(Factory owner) : base(owner, "CRGLSR", "Laser Turret")
    {
      SystemData.MaxShield = 4;
      SystemData.MaxHull = 8;
      CombatData.ImpactDamage = 16;
      RenderData.RadarSize = 0;
      AIData.TargetType = TargetType.NULL;

      MeshData = new MeshData(Engine, Name, @"turbotowers\sm_turbolaser.x");
      DyingMoveData.Kill();

      WeapSystemData.Loadouts = new WeapData[] { new WeapData("", "AI", "ADDON_TURBOLASER", "CRG_LASR", "CRG_LASR", "ADDON_LSR_Y", "ADDON_TURBOLASER") };
    }
  }
}

