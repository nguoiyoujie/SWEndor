using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  internal class mc90TurbolaserATI : Groups.Turbolasers
  {
    internal mc90TurbolaserATI(Factory owner) : base(owner, "MC90LSR", "MC90 Turbolaser Tower")
    {
      SystemData.MaxShield = 60;
      SystemData.MaxHull = 50;
      CombatData.ImpactDamage = 16;
      MoveLimitData.MaxTurnRate = 50f;

      MeshData = new MeshData(Engine, Name, @"turbotowers\mc90_turbolaser.x");
      DyingMoveData.Kill();

      WeapSystemData.Loadouts = new WeapData[] { new WeapData("", "AI", "ADDON_TURBOLASER", "MC90_LASR", "DEFAULT", "ADDON_LSR_R", "ADDON_TURBOLASER") };
    }
  }
}

