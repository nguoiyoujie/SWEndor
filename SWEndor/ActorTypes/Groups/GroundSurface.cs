using SWEndor.Models;

namespace SWEndor.ActorTypes.Groups
{
  public class GroundSurface : ActorTypeInfo
  {
    internal GroundSurface(Factory owner, string id, string name) : base(owner, id, name)
    {
      ImpactDamage = 200;

      Mask = ComponentMask.STATIC_ACTOR;
    }
  }
}

