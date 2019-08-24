using SWEndor.Actors;
using SWEndor.Actors.Data;
using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Groups
{
  public class AddOn : ActorTypeInfo
  {
    internal AddOn(Factory owner, string name) : base(owner, name)
    {
      // Combat
      ExplodeData = new ExplodeData(deathTrigger: DeathExplosionTrigger.ALWAYS);
      CombatData = CombatData.Disabled;
      Armor = ArmorInfo.Immune;

      CullDistance = 3500;

      RadarSize = 1;

      Mask = ComponentMask.STATIC_ACTOR;
      HuntWeight = 1;
    }
  }
}

