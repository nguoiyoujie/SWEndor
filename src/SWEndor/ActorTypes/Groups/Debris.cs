using SWEndor.ActorTypes.Components;
using SWEndor.Models;

namespace SWEndor.ActorTypes.Groups
{
  internal class Debris : ActorTypeInfo
  {
    internal Debris(Factory owner, string id, string name) : base(owner, id, name)
    {
      // Combat
      CombatData = CombatData.Disabled;
      ArmorData = ArmorData.Immune;
      RenderData.CullDistance = 4500;

      AIData.TargetType = TargetType.FLOATING;

      Mask = ComponentMask.DEBRIS;
    }
  }
}

