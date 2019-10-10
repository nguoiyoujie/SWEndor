using SWEndor.ActorTypes.Components;
using SWEndor.Models;

namespace SWEndor.ActorTypes.Instances
{
  public class SDShieldGeneratorATI : Groups.ShieldGenerators
  {
    internal SDShieldGeneratorATI(Factory owner) : base(owner, "SHD", "Shield Generator")
    {
      CombatData.MaxStrength = 105;
      CombatData.ImpactDamage = 300.0f;
      RenderData.RadarSize = 2;

      RenderData.CullDistance = 30000;

      AIData.TargetType |= TargetType.SHIELDGENERATOR;
      RenderData.RadarType = RadarType.HOLLOW_CIRCLE_M;

      RegenData = new RegenData(true, 0, 3.5f, 0, 0.3f);

      MeshData = new MeshData(Name, @"stardestroyer\star_destroyer_energy_pod.x", 0.9f);
      DyingMoveData.Kill();
    }
  }
}

