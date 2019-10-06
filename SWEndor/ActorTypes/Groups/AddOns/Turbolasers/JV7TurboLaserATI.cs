using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class JV7TurboLaserATI : Groups.Turbolasers
  {
    internal JV7TurboLaserATI(Factory owner) : base(owner, "JV7LSR", "Laser Turret")
    {
      CombatData.MaxStrength = 6;
      CombatData.ImpactDamage = 16;
      RenderData.RadarSize = 0;
      AIData.TargetType = TargetType.NULL;

      MeshData = new MeshData(Name, @"turbotowers\xq_turbolaser.x", 0.25f);
      DyingMoveData.Kill();

      Loadouts = new string[] { "JV7_LASR" };
    }
  }
}

