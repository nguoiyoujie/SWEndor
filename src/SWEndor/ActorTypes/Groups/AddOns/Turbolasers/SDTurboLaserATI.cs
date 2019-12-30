using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  internal class SDTurboLaserATI : Groups.Turbolasers
  {
    internal SDTurboLaserATI(Factory owner) : base(owner, "ISD_LSR", "Star Destroyer Turbolaser Tower")
    {
      SystemData.MaxShield = 10;
      SystemData.MaxHull = 6;
      CombatData.ImpactDamage = 16;

      ScoreData = new ScoreData(250, 1250);

      MeshData = new MeshData(Engine, Name, @"turbotowers\star_destroyer_turbolaser.x");
      DyingMoveData.Kill();

      WeapSystemData.Loadouts = new WeapData[] { new WeapData("", "AI", "IMPL_LASR", "IMPL_LASR", "IMPL_LASR", "ADDON_LSR_Y", "ADDON_TURBOLASER") };
    }
  }
}

