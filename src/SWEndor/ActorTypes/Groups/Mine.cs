using SWEndor.ActorTypes.Components;
using SWEndor.Models;

namespace SWEndor.ActorTypes.Groups
{
  internal class Mine : ActorTypeInfo
  {
    internal Mine(Factory owner, string id, string name) : base(owner, id, name)
    {
      // Combat
      Explodes = new ExplodeData[] { new ExplodeData("EXPL00", 1, 1, ExplodeTrigger.ON_DEATH) };
      CombatData = CombatData.DefaultShip;
      ArmorData = ArmorData.Default;

      ScoreData = new ScoreData(0, 1000);
      RenderData.CullDistance = 5500;

      AIData.Attack_AngularDelta = 360f;
      AIData.Attack_HighAccuracyAngularDelta = 360f;

      AIData.TargetType |= TargetType.ADDON;
      RenderData.RadarType = RadarType.HOLLOW_CIRCLE_S;

      Mask |= ComponentMask.HAS_AI;

      RenderData.RadarSize = 1;
    }
  }
}

