using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class SDShieldGeneratorATI : Groups.AddOn
  {
    internal SDShieldGeneratorATI(Factory owner) : base(owner, "Star Destroyer Shield Generator")
    {
      // Combat
      IsCombatObject = true;
      IsSelectable = true;
      IsDamage = false;
      CollisionEnabled = true;

      MaxStrength = 105;
      ImpactDamage = 300.0f;
      RadarSize = 2;

      CullDistance = 30000;

      Score_perStrength = 75;
      Score_DestroyBonus = 2500;

      TargetType |= TargetType.SHIELDGENERATOR;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"stardestroyer\star_destroyer_energy_pod.x");
    }


    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.DyingMovement = DyingKillInfo.Instance;

      ainfo.CombatInfo.CollisionDamageModifier = 100;
      ainfo.RegenerationInfo.AllowRegen = false;
      ainfo.RegenerationInfo.ParentRegenRate = 3.5f;
      ainfo.RegenerationInfo.RelativeRegenRate = 0.3f;

      ainfo.Scale *= 0.9f;
    }
  }
}

