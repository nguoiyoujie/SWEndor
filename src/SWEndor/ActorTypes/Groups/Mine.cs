using SWEndor.ActorTypes.Components;
using SWEndor.Models;

namespace SWEndor.ActorTypes.Groups
{
  internal class Mine : ActorTypeInfo
  {
    internal Mine(Factory owner, string id, string name) : base(owner, id, name)
    {
      // Combat
      ExplodeSystemData.Explodes = new ExplodeData[] { new ExplodeData("EXPL00", 1, 1, ExplodeTrigger.ON_DEATH) };
      CombatData = CombatData.DefaultShip;
      ArmorData = ArmorData.Default;

      ScoreData = new ScoreData(0, 1000);
      RenderData.CullDistance = 5500;
      DyingMoveData.Kill();

      AIData.Attack_AngularDelta = 360f;
      AIData.Attack_HighAccuracyAngularDelta = 360f;

      AIData.TargetType = TargetType.MUNITION;
      RenderData.RadarType = RadarType.HOLLOW_CIRCLE_S;
      RenderData.RadarSize = 1;

      MoveLimitData.ZNormFrac = 0;

      Mask = ComponentMask.ACTOR;
      AIData.HuntWeight = 1;
    }
  }
}

