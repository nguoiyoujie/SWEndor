using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  internal class NebulonBTurboLaserATI : Groups.Turbolasers
  {
    internal NebulonBTurboLaserATI(Factory owner) : base(owner, "NEBLLSR", "Nebulon B Turbolaser Tower")
    {
      SystemData.MaxShield = 135;
      CombatData.ImpactDamage = 16;

      MeshData = new MeshData(Engine, Name, @"turbotowers\nebulonb_turbolaser.x");
      DyingMoveData.Kill();

      Loadouts = new WeapData[] { new WeapData("", "AI", "ADDON_TURBOLASER", "NEBL_LASR", "DEFAULT", "ADDON_LSR_R", "ADDON_TURBOLASER") };
    }
  }
}

