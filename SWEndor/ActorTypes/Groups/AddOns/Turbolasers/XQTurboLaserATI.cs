using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class XQTurboLaserATI : Groups.Turbolasers
  {
    internal XQTurboLaserATI(Factory owner) : base(owner, "XQLSR", "Plaform Turbolaser Tower")
    {
      CombatData.MaxStrength = 125;
      CombatData.ImpactDamage = 16;

      MeshData = new MeshData(Name, @"turbotowers\xq_turbolaser.x");
      DyingMoveData.Kill();

      Loadouts = new string[] { "XQ_LASR" };
    }
  }
}

