using SWEndor.Actors.Data;
using SWEndor.Actors.Models;
using SWEndor.ActorTypes.Components;
using SWEndor.Models;

namespace SWEndor.ActorTypes.Groups
{
  public class Turbolasers : AddOn
  {
    internal Turbolasers(Factory owner, string name) : base(owner, name)
    {
      // Combat
      CombatData = CombatData.DefaultShip;
      ArmorData = ArmorData.Default;
      Explodes = new ExplodeData[] {
        new ExplodeData("ExpS00", 1, 5, ExplodeTrigger.ON_DEATH)
      };

      ScoreData = new ScoreData(250, 1250);

      AIData.Attack_AngularDelta = 360f;
      AIData.Attack_HighAccuracyAngularDelta = 360f;

      AIData.TargetType |= TargetType.ADDON;
      RenderData.RadarType = RadarType.HOLLOW_CIRCLE_S;

      Mask |= ComponentMask.HAS_AI;

      RenderData.RadarSize = 1;
    }
  }
}

