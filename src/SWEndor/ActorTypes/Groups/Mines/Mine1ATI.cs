using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  internal class Mine1ATI : Groups.Mine
  {
    internal Mine1ATI(Factory owner) : base(owner, "MINE1", "Mine A")
    {
      SystemData.MaxShield = 0;
      SystemData.MaxHull = 1;
      CombatData.ImpactDamage = 2;

      MeshData = new MeshData(Engine, Name, @"mines\mine1.x");
      DyingMoveData.Kill();

      Loadouts = new WeapData[] { new WeapData("", "AI", "ADDON_TURBOLASER", "DEFAULT", "MIN1_LASR", "ADDON_LSR_Y", "ADDON_TURBOLASER") };
    }
  }
}

