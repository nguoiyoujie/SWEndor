using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class ISDShieldGeneratorATI : Groups.ShieldGenerators
  {
    internal ISDShieldGeneratorATI(Factory owner) : base(owner, "Imperial Star Destroyer Shield Generator")
    {
      MaxStrength = 105;
      ImpactDamage = 300.0f;
      RadarSize = 2;

      CullDistance = 30000;

      Score_perStrength = 75;
      Score_DestroyBonus = 2500;

      TargetType |= TargetType.SHIELDGENERATOR;
      RadarType = RadarType.HOLLOW_CIRCLE_M;

      RegenData = new RegenInfo { NoRegen = true, ParentRegenRate = 12f, SiblingRegenRate = 0.5f };


      SourceMeshPath = Path.Combine(Globals.ModelPath, @"stardestroyer\star_destroyer_energy_pod.x");
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.DyingMoveComponent = DyingKill.Instance;
    }
  }
}

