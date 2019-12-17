using SWEndor.ActorTypes.Components;
using SWEndor.Models;

namespace SWEndor.ActorTypes.Instances
{
  internal class SDShieldGeneratorATI : Groups.ShieldGenerators
  {
    internal SDShieldGeneratorATI(Factory owner) : base(owner, "SHD", "Shield Generator")
    {
      SystemData.MaxShield = 25;
      SystemData.MaxHull = 90;
      CombatData.ImpactDamage = 300.0f;
      RenderData.RadarSize = 2;

      RenderData.CullDistance = 50000;

      AIData.TargetType |= TargetType.SHIELDGENERATOR;
      RenderData.RadarType = RadarType.HOLLOW_CIRCLE_M;

      RegenData = new RegenData(false, 0, 6, 0, 0.6f);

      MeshData = new MeshData(Engine, Name, @"stardestroyer\star_destroyer_energy_pod.x", 0.75f);
      DyingMoveData.Kill();
    }
  }
}

