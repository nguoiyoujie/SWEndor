using SWEndor.Actors.Data;
using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Groups
{
  public class ShieldGenerators : AddOn
  {
    internal ShieldGenerators(Factory owner, string name) : base(owner, name)
    {
      // Combat
      CombatData = CombatData.DefaultFighter;
      Armor = new ArmorInfo() { Light = 1, Hull = 100 };

      TargetType |= TargetType.SHIELDGENERATOR;
      RadarType = RadarType.HOLLOW_CIRCLE_M;

      RadarSize = 2;
      HuntWeight = 3;
    }
  }
}

