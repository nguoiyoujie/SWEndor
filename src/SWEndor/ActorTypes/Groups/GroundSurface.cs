using SWEndor.Models;

namespace SWEndor.ActorTypes.Groups
{
  internal class GroundSurface : ActorTypeInfo
  {
    internal GroundSurface(Factory owner, string id, string name) : base(owner, id, name)
    {
      CombatData.ImpactDamage = 200;

      Mask = ComponentMask.STATIC_ACTOR;
    }
  }
}

