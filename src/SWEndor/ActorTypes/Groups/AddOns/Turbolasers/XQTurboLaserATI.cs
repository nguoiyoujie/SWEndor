using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  internal class XQTurboLaserATI : Groups.Turbolasers
  {
    internal XQTurboLaserATI(Factory owner) : base(owner, "XQLSR", "Plaform Turbolaser Tower")
    {
      SystemData.MaxShield = 50;
      SystemData.MaxHull = 40;
      CombatData.ImpactDamage = 16;

      MeshData = new MeshData(Engine, Name, @"turbotowers\xq_turbolaser.x");
      DyingMoveData.Kill();

      Loadouts = new string[] { "XQ_LASR" };
    }
  }
}

