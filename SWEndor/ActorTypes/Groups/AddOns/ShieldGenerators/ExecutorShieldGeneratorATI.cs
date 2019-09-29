using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class ExecutorShieldGeneratorATI : Groups.ShieldGenerators
  {
    internal ExecutorShieldGeneratorATI(Factory owner) : base(owner, "Executor Super Star Destroyer Shield Generator")
    {
      // Combat
      MaxStrength = 150.0f;
      ImpactDamage = 500.0f;
      RenderData.RadarSize = 2;

      RenderData.CullDistance = 30000;

      ScoreData = new ScoreData(100, 5000);
      DyingMoveData.Kill();

      AIData.TargetType |= TargetType.SHIELDGENERATOR;
      RenderData.RadarType = RadarType.HOLLOW_CIRCLE_M;

      RegenData = new RegenData(false, 0, 40, 0, 0.1f);
      MeshData = new MeshData(Name, @"executor\executor_energy_pod.x");
    }
  }
}

