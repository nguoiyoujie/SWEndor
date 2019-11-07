using SWEndor.ActorTypes.Components;
using SWEndor.Models;

namespace SWEndor.ActorTypes.Groups
{
  public class ShieldGenerators : AddOn
  {
    internal ShieldGenerators(Factory owner, string id, string name) : base(owner, id, name)
    {
      // Combat
      CombatData = CombatData.DefaultFighter;
      ArmorData = new ArmorData(1, 100);

      ScoreData = new ScoreData(75, 2500);

      AIData.TargetType |= TargetType.SHIELDGENERATOR;
      RenderData.RadarType = RadarType.HOLLOW_CIRCLE_M;

      RenderData.RadarSize = 2;
      AIData.HuntWeight = 3;
    }
  }
}

