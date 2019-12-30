using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  internal class SDMissilePodATI : Groups.Turbolasers
  {
    internal SDMissilePodATI(Factory owner) : base(owner, "IMPLMPOD", "Star Destroyer Missile Pod")
    {
      SystemData.MaxShield = 10; //32
      SystemData.MaxHull = 20;
      CombatData.ImpactDamage = 16;

      MeshData = new MeshData(Engine, Name, @"turbotowers\star_destroyer_missilepod.x");
      DyingMoveData.Kill();

      WeapSystemData.Loadouts = new WeapData[] { new WeapData("", "AI", "NO_AUTOAIM", "IMPL_MISL", "IMPL_MISL", "ADDON_MISL", "ADDON_MISSILE") };
    }
  }
}

