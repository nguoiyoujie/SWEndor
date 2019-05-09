using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Actors.Data;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class ExecutorShieldGeneratorATI : Groups.ShieldGenerators
  {
    internal ExecutorShieldGeneratorATI(Factory owner) : base(owner, "Executor Super Star Destroyer Shield Generator")
    {
      // Combat
      MaxStrength = 150.0f;
      ImpactDamage = 500.0f;
      RadarSize = 2;

      CullDistance = 30000;

      Score_perStrength = 100;
      Score_DestroyBonus = 5000;

      TargetType |= TargetType.SHIELDGENERATOR;
      RadarType = RadarType.HOLLOW_CIRCLE_M;

      RegenData = new RegenData { ParentRegenRate = 40f, SiblingRegenRate = 0.1f };

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"executor\executor_energy_pod.x");
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.DyingMoveComponent = DyingKill.Instance;
    }
  }
}

