using SWEndor.ActorTypes.Components;
using SWEndor.Models;

namespace SWEndor.ActorTypes.Instances
{
  internal class ISDShieldGeneratorATI : Groups.ShieldGenerators
  {
    internal ISDShieldGeneratorATI(Factory owner) : base(owner, "IMPLSHD", "Shield Generator")
    {
      SystemData.MaxShield = 25;
      SystemData.MaxHull = 80;
      CombatData.ImpactDamage = 300.0f;
      RenderData.RadarSize = 2;

      RenderData.CullDistance = 30000;
      DyingMoveData.Kill();

      AIData.TargetType |= TargetType.SHIELDGENERATOR;
      RenderData.RadarType = RadarType.HOLLOW_CIRCLE_M;

      RegenData = new RegenData(false, 0, 15, 0, 1);


      MeshData = new MeshData(Name, @"stardestroyer\star_destroyer_energy_pod.x");
    }
  }
}

