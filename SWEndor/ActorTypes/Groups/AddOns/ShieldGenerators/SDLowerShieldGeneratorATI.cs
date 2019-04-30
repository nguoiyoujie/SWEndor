using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class SDLowerShieldGeneratorATI : Groups.AddOn
  {
    internal SDLowerShieldGeneratorATI(Factory owner) : base(owner, "Star Destroyer Lower Shield Generator")
    {
      // Combat
      IsCombatObject = true;
      IsSelectable = true;
      IsDamage = false;
      CollisionEnabled = true;

      MaxStrength = 90;
      ImpactDamage = 300.0f;
      RadarSize = 2.5f;

      CullDistance = 8000;

      Score_perStrength = 75;
      Score_DestroyBonus = 2500;

      TargetType |= TargetType.SHIELDGENERATOR;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"stardestroyer\star_destroyer_lower_energy_pod.x");
    }


    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.DyingMoveComponent = DyingKill.Instance;

      ainfo.CombatInfo.CollisionDamageModifier = 100;
      ainfo.RegenerationInfo.AllowRegen = false;
      ainfo.RegenerationInfo.ParentRegenRate = 20f;
      ainfo.RegenerationInfo.RelativeRegenRate = 0.5f;
    }
  }
}

