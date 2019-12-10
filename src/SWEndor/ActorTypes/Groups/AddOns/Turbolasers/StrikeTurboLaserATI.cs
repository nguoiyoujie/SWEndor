using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  internal class StrikeTurboLaserATI : Groups.Turbolasers
  {
    internal StrikeTurboLaserATI(Factory owner) : base(owner, "STRKLSR", "Strike-class Turbolaser Tower")
    {
      SystemData.MaxShield = 20;
      SystemData.MaxHull = 40;
      CombatData.ImpactDamage = 16;

      MeshData = new MeshData(Engine, Name, @"turbotowers\acclamator_turbolaser.x");
      DyingMoveData.Kill();

      Loadouts = new WeapData[] { new WeapData("", "AI", "ADDON_TURBOLASER", "STRK_LASR", "STRK_LASR", "ADDON_LSR_Y", "ADDON_TURBOLASER") };
    }
  }
}

