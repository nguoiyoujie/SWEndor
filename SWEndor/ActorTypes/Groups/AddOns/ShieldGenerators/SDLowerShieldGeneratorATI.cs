using SWEndor.ActorTypes.Components;
using SWEndor.Models;

namespace SWEndor.ActorTypes.Instances
{
  public class SDLowerShieldGeneratorATI : Groups.ShieldGenerators
  {
    internal SDLowerShieldGeneratorATI(Factory owner) : base(owner, "LSHD", "Shield Generator")
    {
      CombatData.MaxStrength = 90;
      CombatData.ImpactDamage = 300.0f;
      RenderData.RadarSize = 2.5f;
      DyingMoveData.Kill();

      RenderData.CullDistance = 8000;

      AIData.TargetType |= TargetType.SHIELDGENERATOR;
      RenderData.RadarType = RadarType.HOLLOW_CIRCLE_M;

      RegenData = new RegenData(true, 0, 20, 0, 0.5f);

      MeshData = new MeshData(Name, @"stardestroyer\star_destroyer_lower_energy_pod.x");
    }
  }
}

