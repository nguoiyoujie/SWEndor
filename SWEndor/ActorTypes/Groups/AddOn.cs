using SWEndor.Actors;
using SWEndor.Actors.Data;

namespace SWEndor.ActorTypes.Groups
{
  public class AddOn : ActorTypeInfo
  {
    internal AddOn(Factory owner, string name) : base(owner, name)
    {
      // Combat
      ExplodeData = new ExplodeData(deathTrigger: DeathExplosionTrigger.ALWAYS);
      CombatData = CombatData.Disabled;

      CullDistance = 3500;

      RadarSize = 1;

      Mask = ComponentMask.STATIC_ACTOR;
    }
  }
}

