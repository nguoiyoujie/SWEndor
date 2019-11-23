using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  internal class ACTurboLaserATI : Groups.Turbolasers
  {
    internal ACTurboLaserATI(Factory owner) : base(owner, "ACCLLSR", "Acclamator Turbolaser Tower")
    {
      SystemData.MaxShield = 20;
      SystemData.MaxHull = 50;
      CombatData.ImpactDamage = 16;

      MeshData = new MeshData(Engine, Name, @"turbotowers\acclamator_turbolaser.x");
      DyingMoveData.Kill();

      Loadouts = new WeapData[] { new WeapData("", "AI", "ADDON_TURBOLASER", "ACCL_LASR", "ACCL_LASR", "ADDON_LSR_Y", "ADDON_TURBOLASER") };
    }
  }
}

