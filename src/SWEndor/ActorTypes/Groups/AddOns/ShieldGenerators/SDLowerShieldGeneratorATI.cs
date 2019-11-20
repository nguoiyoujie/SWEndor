using SWEndor.ActorTypes.Components;
using SWEndor.Models;

namespace SWEndor.ActorTypes.Instances
{
  internal class SDLowerShieldGeneratorATI : Groups.ShieldGenerators
  {
    internal SDLowerShieldGeneratorATI(Factory owner) : base(owner, "LSHD", "Shield Generator")
    {
      SystemData.MaxShield = 30;
      SystemData.MaxHull = 60;
      CombatData.ImpactDamage = 300.0f;
      RenderData.RadarSize = 2.5f;
      DyingMoveData.Kill();

      RenderData.CullDistance = 8000;

      AIData.TargetType |= TargetType.SHIELDGENERATOR;
      RenderData.RadarType = RadarType.HOLLOW_CIRCLE_M;

      RegenData = new RegenData(true, 0, 20, 0, 1);

      MeshData = new MeshData(Engine, Name, @"stardestroyer\star_destroyer_lower_energy_pod.x");
    }
  }
}

