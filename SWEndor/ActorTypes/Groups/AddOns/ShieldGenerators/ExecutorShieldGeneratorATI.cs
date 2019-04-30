using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class ExecutorShieldGeneratorATI : Groups.AddOn
  {
    internal ExecutorShieldGeneratorATI(Factory owner) : base(owner, "Executor Super Star Destroyer Shield Generator")
    {
      // Combat
      IsCombatObject = true;
      IsSelectable = true;
      IsDamage = false;
      CollisionEnabled = true;

      MaxStrength = 150.0f;
      ImpactDamage = 500.0f;
      RadarSize = 2;

      CullDistance = 30000;

      Score_perStrength = 100;
      Score_DestroyBonus = 5000;

      TargetType |= TargetType.SHIELDGENERATOR;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"executor\executor_energy_pod.x");
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.DyingMoveComponent = DyingKill.Instance;

      ainfo.CombatInfo.CollisionDamageModifier = 100;
      ainfo.RegenerationInfo.ParentRegenRate = 40f;
      ainfo.RegenerationInfo.RelativeRegenRate = 0.1f;
    }
  }
}

