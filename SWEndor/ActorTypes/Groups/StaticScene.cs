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
      Armor = ArmorInfo.Immune;

      MaxStrength = 100000.0f;
      MaxSpeed = 0.0f;
      MinSpeed = 0.0f;
      MaxSpeedChangeRate = 0.0f;
      MaxTurnRate = 0.0f;
      EnableDistanceCull = false;

      Score_perStrength = 0;
      Score_DestroyBonus = 0;

      Mask = ComponentMask.STATIC_PROP;
    }
  }
}

