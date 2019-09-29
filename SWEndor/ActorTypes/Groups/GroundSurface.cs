using SWEndor.Models;

namespace SWEndor.ActorTypes.Groups
{
  public class GroundSurface : ActorTypeInfo
  {
    internal GroundSurface(Factory owner, string name) : base(owner, name)
    {
      ImpactDamage = 200;

      Mask = ComponentMask.STATIC_ACTOR;
    }
  }
}

