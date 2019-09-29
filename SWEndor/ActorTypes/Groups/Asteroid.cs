using SWEndor.Actors.Data;
using SWEndor.ActorTypes.Components;
using SWEndor.Models;

namespace SWEndor.ActorTypes.Groups
{
  public class Asteroid : ActorTypeInfo
  {
    internal Asteroid(Factory owner, string name) : base(owner, name)
    {
      // Combat
      CombatData = CombatData.Disabled;
      ArmorData = ArmorData.Immune;
      Explodes = new ExplodeData[] { new ExplodeData("ExpL00", 1, 1, ExplodeTrigger.ON_DEATH) };

      RenderData.CullDistance = 4500;

      RenderData.RadarType = RadarType.HOLLOW_CIRCLE_M;

      Mask = ComponentMask.MINDLESS_ACTOR;
    }
  }
}

