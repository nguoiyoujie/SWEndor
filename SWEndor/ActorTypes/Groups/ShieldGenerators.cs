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
      ArmorData = new ArmorData(1, 100);

      ScoreData = new ScoreData(75, 2500);

      AIData.TargetType |= TargetType.SHIELDGENERATOR;
      RenderData.RadarType = RadarType.HOLLOW_CIRCLE_M;

      RenderData.RadarSize = 2;
      AIData.HuntWeight = 3;
    }
  }
}

