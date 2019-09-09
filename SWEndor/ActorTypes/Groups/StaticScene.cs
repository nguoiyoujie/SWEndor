using SWEndor.Actors;
using SWEndor.Actors.Data;
using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Groups
{
  public class StaticScene : ActorTypeInfo
  {
    internal StaticScene(Factory owner, string name) : base(owner, name)
    {
      // Combat
      CombatData = CombatData.Disabled;
      ArmorData = ArmorData.Immune;

      MaxStrength = 100000.0f;
      MoveLimitData.MaxSpeed = 0.0f;
      MoveLimitData.MinSpeed = 0.0f;
      MoveLimitData.MaxSpeedChangeRate = 0.0f;
      MoveLimitData.MaxTurnRate = 0.0f;

      RenderData.CullDistance = -1;

      Mask = ComponentMask.STATIC_PROP;
    }
  }
}

