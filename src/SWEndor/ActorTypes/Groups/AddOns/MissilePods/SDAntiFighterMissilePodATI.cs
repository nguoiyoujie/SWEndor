using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  internal class SDAntiFighterMissilePodATI : Groups.Turbolasers
  {
    internal SDAntiFighterMissilePodATI(Factory owner) : base(owner, "IMPLMPOD2", "Star Destroyer Missile Pod")
    {
      SystemData.MaxShield = 10; //32
      SystemData.MaxHull = 20;
      CombatData.ImpactDamage = 16;

      MeshData = new MeshData(Engine, Name, @"turbotowers\star_destroyer_anti-fighter_missilepod.x");
      DyingMoveData.Kill();

      Loadouts = new WeapData[] { new WeapData("", "AI", "NO_AUTOAIM", "IMPL_MISL2", "IMPL_MISL2", "ADDON_MISL", "MISL_VS_ANY") };
    }
  }
}

