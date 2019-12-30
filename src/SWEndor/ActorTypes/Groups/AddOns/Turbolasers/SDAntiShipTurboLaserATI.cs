using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  internal class SDAntiShipTurboLaserATI : Groups.Turbolasers
  {
    internal SDAntiShipTurboLaserATI(Factory owner) : base(owner, "ISD_HLSR", "Star Destroyer Heavy Turbolaser Tower")
    {
      SystemData.MaxShield = 8;
      SystemData.MaxHull = 4;
      CombatData.ImpactDamage = 16;

      ScoreData = new ScoreData(250, 1250);

      MeshData = new MeshData(Engine, Name, @"turbotowers\star_destroyer_anti-ship_turbolaser.x");
      DyingMoveData.Kill();

      WeapSystemData.Loadouts = new WeapData[] { new WeapData("", "AI", "IMPL_3LSR", "IMPL_3LSR", "IMPL_3LSR", "ADDON_LSR_G3", "IMPL_3LSR") };
    }
  }
}

