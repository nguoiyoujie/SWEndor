using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class ISDShieldGeneratorATI : Groups.ShieldGenerators
  {
    internal ISDShieldGeneratorATI(Factory owner) : base(owner, "IMPLSHD", "Shield Generator")
    {
      CombatData.MaxStrength = 105;
      CombatData.ImpactDamage = 300.0f;
      RenderData.RadarSize = 2;

      RenderData.CullDistance = 30000;
      DyingMoveData.Kill();

      AIData.TargetType |= TargetType.SHIELDGENERATOR;
      RenderData.RadarType = RadarType.HOLLOW_CIRCLE_M;

      RegenData = new RegenData(true, 0, 12, 0, 0.5f);


      MeshData = new MeshData(Name, @"stardestroyer\star_destroyer_energy_pod.x");
    }
  }
}

