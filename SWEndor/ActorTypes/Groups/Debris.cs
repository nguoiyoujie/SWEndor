using SWEndor.Actors;
using SWEndor.Actors.Data;
using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Groups
{
  public class Debris : ActorTypeInfo
  {
    internal Debris(Factory owner, string name) : base(owner, name)
    {
      // Combat
      CombatData = CombatData.Disabled;
      Armor = ArmorInfo.Immune;
      CullDistance = 4500f;

      Mask = ComponentMask.DEBRIS;
    }
  }
}

