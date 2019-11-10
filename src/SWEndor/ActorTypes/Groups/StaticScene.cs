using SWEndor.ActorTypes.Components;
using SWEndor.Models;

namespace SWEndor.ActorTypes.Groups
{
  internal class StaticScene : ActorTypeInfo
  {
    internal StaticScene(Factory owner, string id, string name) : base(owner, id, name)
    {
      // Combat
      CombatData = CombatData.Disabled;
      ArmorData = ArmorData.Immune;

      SystemData.MaxShield = 100000.0f;
      MoveLimitData.MaxSpeed = 0.0f;
      MoveLimitData.MinSpeed = 0.0f;
      MoveLimitData.MaxSpeedChangeRate = 0.0f;
      MoveLimitData.MaxTurnRate = 0.0f;

      RenderData.CullDistance = -1;

      Mask = ComponentMask.STATIC_PROP;
    }
  }
}

