using SWEndor.ActorTypes.Components;
using SWEndor.Models;

namespace SWEndor.ActorTypes.Groups
{
  internal class Asteroid : ActorTypeInfo
  {
    internal Asteroid(Factory owner, string id, string name) : base(owner, id, name)
    {
      // Combat
      CombatData = CombatData.Disabled;
      ArmorData = ArmorData.Immune;
      Explodes = new ExplodeData[] { new ExplodeData("EXPL00", 1, 1, ExplodeTrigger.ON_DEATH) };

      RenderData.CullDistance = 6000;

      RenderData.RadarType = RadarType.HOLLOW_CIRCLE_M;

      Mask = ComponentMask.MINDLESS_ACTOR;
    }
  }
}

