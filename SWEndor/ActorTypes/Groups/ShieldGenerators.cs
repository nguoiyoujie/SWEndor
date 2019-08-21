using SWEndor.Actors;
using SWEndor.Actors.Data;

namespace SWEndor.ActorTypes.Groups
{
  public class ShieldGenerators : AddOn
  {
    internal ShieldGenerators(Factory owner, string name) : base(owner, name)
    {
      // Combat
      CombatData = CombatData.DefaultFighter;
      Armor = new ActorInfo.ArmorModel() { Light = 1, Hull = 100 };

      TargetType |= TargetType.SHIELDGENERATOR;
      RadarType = RadarType.HOLLOW_CIRCLE_M;

      RadarSize = 2;
    }
  }
}

