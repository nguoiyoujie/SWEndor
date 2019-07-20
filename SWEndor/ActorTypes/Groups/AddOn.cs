using SWEndor.Actors;
using SWEndor.Actors.Data;
using SWEndor.Actors.Traits;

namespace SWEndor.ActorTypes.Groups
{
  public class AddOn : ActorTypeInfo
  {
    internal AddOn(Factory owner, string name) : base(owner, name)
    {
      // Combat
      Explodes = new ExplodeInfo[] { new ExplodeInfo("ExpL00", 1, 1, ExplodeTrigger.ON_DEATH) };
      CombatData = CombatData.Disabled;

      CullDistance = 3500;

      RadarSize = 1;
      TargetType = TargetType.ADDON;

      Mask = ComponentMask.STATIC_ACTOR;
    }
  }
}

