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
      RenderData.RadarSize = 2;

      RenderData.CullDistance = 30000;
      
      AIData.TargetType |= TargetType.SHIELDGENERATOR;
      RenderData.RadarType = RadarType.HOLLOW_CIRCLE_M;

      RegenData = new RegenData(true, 0, 12, 0, 0.5f);


      MeshData = new MeshData(Name, @"stardestroyer\star_destroyer_energy_pod.x");
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.DyingMoveComponent = DyingKill.Instance;
    }
  }
}

