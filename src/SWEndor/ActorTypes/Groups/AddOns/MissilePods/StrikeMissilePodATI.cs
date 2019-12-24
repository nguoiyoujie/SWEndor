using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  internal class StrikeMissilePodATI : Groups.Turbolasers
  {
    internal StrikeMissilePodATI(Factory owner) : base(owner, "STRKMPOD", "Strike-class Missile Pod")
    {
      SystemData.MaxShield = 20;
      SystemData.MaxHull = 30;
      CombatData.ImpactDamage = 16;

      MeshData = new MeshData(Engine, Name, @"turbotowers\anti-fighter_missilepod.x");
      DyingMoveData.Kill();

      Loadouts = new WeapData[] { new WeapData("", "AI", "NO_AUTOAIM", "STRK_MISL", "STRK_MISL", "ADDON_MISL", "STRK_MISL") };
    }
  }
}

