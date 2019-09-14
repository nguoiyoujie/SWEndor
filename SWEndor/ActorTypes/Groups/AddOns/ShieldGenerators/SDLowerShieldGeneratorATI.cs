using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class SDLowerShieldGeneratorATI : Groups.ShieldGenerators
  {
    internal SDLowerShieldGeneratorATI(Factory owner) : base(owner, "Star Destroyer Lower Shield Generator")
    {
      MaxStrength = 90;
      ImpactDamage = 300.0f;
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

