using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  internal class Mine3ATI : Groups.Mine
  {
    internal Mine3ATI(Factory owner) : base(owner, "MINE3", "Mine")
    {
      SystemData.MaxShield = 0;
      SystemData.MaxHull = 1;
      CombatData.ImpactDamage = 2;

      MeshData = new MeshData(Engine, Name, @"mines\mine3.x");
      DyingMoveData.Kill();

      Loadouts = new WeapData[] { new WeapData("", "AI", "ADDON_TURBOLASER", "ACCL_LASR", "ACCL_LASR", "ADDON_LSR_Y", "ADDON_TURBOLASER") };
    }
  }
}

