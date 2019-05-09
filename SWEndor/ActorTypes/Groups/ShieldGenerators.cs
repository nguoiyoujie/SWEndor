using SWEndor.Actors;
using SWEndor.Actors.Data;

namespace SWEndor.ActorTypes.Groups
{
  public class ShieldGenerators : AddOn
  {
    internal ShieldGenerators(Factory owner, string name) : base(owner, name)
    {
      // Combat
      CombatData = new CombatData(true, true, 1, 100);

      TargetType |= TargetType.SHIELDGENERATOR;
      RadarType = RadarType.HOLLOW_CIRCLE_M;

      RadarSize = 2;
    }
  }
}

