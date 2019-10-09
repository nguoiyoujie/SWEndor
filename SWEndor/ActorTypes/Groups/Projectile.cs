using SWEndor.Actors;
using SWEndor.Actors.Models;
using SWEndor.ActorTypes.Components;
using SWEndor.Models;

namespace SWEndor.ActorTypes.Groups
{
  public class Projectile : ActorTypeInfo
  {
    internal Projectile(Factory owner, string id, string name) : base(owner, id, name)
    {
      // Combat
      CombatData = CombatData.Disabled;
      ArmorData = ArmorData.Immune;
      Explodes = new ExplodeData[]
      {
        new ExplodeData("EXPS00", 1, 1, ExplodeTrigger.ON_DEATH | ExplodeTrigger.ONLY_WHEN_DYINGTIME_NOT_EXPIRED)
      };
      DyingMoveData.Kill();

      RenderData.CullDistance = 12000;

      RenderData.RadarType = RadarType.TRAILLINE;
      RenderData.RadarSize = 1;

      AIData.Move_CloseEnough = 0;
      AIData.TargetType = TargetType.LASER;
      MoveLimitData.MaxSecondOrderTurnRateFrac = 0.5f;

      Mask = ComponentMask.LASER_PROJECTILE;
      CombatData.DamageType = DamageType.NORMAL;
    }
  }
}


