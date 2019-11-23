using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  internal class NebulonBMissilePodATI : Groups.Turbolasers
  {
    internal NebulonBMissilePodATI(Factory owner) : base(owner, "NEBLMPOD", "Nebulon B Missile Pod")
    {
      SystemData.MaxShield = 40; //32
      SystemData.MaxHull = 100;
      CombatData.ImpactDamage = 16;

      MeshData = new MeshData(Engine, Name, @"turbotowers\nebulonb_missilepod.x");
      DyingMoveData.Kill();

      Loadouts = new WeapData[] { new WeapData("", "AI", "NO_AUTOAIM", "NEBL_MISL", "DEFAULT", "ADDON_MISL", "ADDON_MISSILE") };
    }
  }
}

