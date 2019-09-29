using SWEndor.Actors.Data;
using SWEndor.ActorTypes.Components;
using SWEndor.Models;

namespace SWEndor.ActorTypes.Groups
{
  public class Debris : ActorTypeInfo
  {
    internal Debris(Factory owner, string name) : base(owner, name)
    {
      // Combat
      CombatData = CombatData.Disabled;
      ArmorData = ArmorData.Immune;
      RenderData.CullDistance = 4500f;

      Mask = ComponentMask.DEBRIS;
    }
  }
}

