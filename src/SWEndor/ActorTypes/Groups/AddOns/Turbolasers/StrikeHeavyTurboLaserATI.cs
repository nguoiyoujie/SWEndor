using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  internal class StrikeHeavyTurboLaserATI : Groups.Turbolasers
  {
    internal StrikeHeavyTurboLaserATI(Factory owner) : base(owner, "STRKHLSR", "Strike-class Heavy Turbolaser Tower")
    {
      SystemData.MaxShield = 15;
      SystemData.MaxHull = 40;
      CombatData.ImpactDamage = 16;

      MeshData = new MeshData(Engine, Name, @"turbotowers\star_destroyer_anti-ship_turbolaser.x");
      DyingMoveData.Kill();

      Loadouts = new WeapData[] { new WeapData("", "AI", "STRK_HLSR", "STRK_HLSR", "STRK_HLSR", "ADDON_LSR_G", "STRK_HLSR") };
    }
  }
}

