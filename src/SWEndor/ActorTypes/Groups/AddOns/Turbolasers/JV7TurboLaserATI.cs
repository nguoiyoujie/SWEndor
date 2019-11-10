using SWEndor.ActorTypes.Components;
using SWEndor.Models;

namespace SWEndor.ActorTypes.Instances
{
  internal class JV7TurboLaserATI : Groups.Turbolasers
  {
    internal JV7TurboLaserATI(Factory owner) : base(owner, "JV7LSR", "Laser Turret")
    {
      SystemData.MaxShield = 4;
      SystemData.MaxHull = 8;
      CombatData.ImpactDamage = 16;
      RenderData.RadarSize = 0;
      AIData.TargetType = TargetType.NULL;

      MeshData = new MeshData(Name, @"turbotowers\xq_turbolaser.x", 0.25f);
      DyingMoveData.Kill();

      Loadouts = new string[] { "JV7_LASR" };
    }
  }
}

