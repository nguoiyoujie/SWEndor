using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Actors.Data;
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
      RadarSize = 2.5f;

      CullDistance = 8000;

      Score_perStrength = 75;
      Score_DestroyBonus = 2500;

      TargetType |= TargetType.SHIELDGENERATOR;
      RadarType = RadarType.HOLLOW_CIRCLE_M;

      RegenData = new RegenInfo { NoRegen = true, ParentRegenRate = 20f, SiblingRegenRate = 0.5f };

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"stardestroyer\star_destroyer_lower_energy_pod.x");
    }


    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.DyingMoveComponent = DyingKill.Instance;
    }
  }
}

