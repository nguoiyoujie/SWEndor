using SWEndor.Actors;
using SWEndor.ActorTypes.Components;
using SWEndor.Models;

namespace SWEndor.ProjectileTypes.Groups
{
  public class Projectile : ProjectileTypeInfo
  {
    internal Projectile(Factory owner, string id, string name) : base(owner, id, name)
    {
      // Combat
      CombatData = CombatData.DefaultProjectile;
      Explodes = new ExplodeData[]
      {
        new ExplodeData("EXPS00", 1, 1, ExplodeTrigger.ON_DEATH | ExplodeTrigger.ONLY_WHEN_DYINGTIME_NOT_EXPIRED)
      };

      RenderData.CullDistance = 12000;

      RenderData.RadarType = RadarType.TRAILLINE;
      RenderData.RadarSize = 1;

      //AIData.Move_CloseEnough = 0;
      MoveLimitData.MaxSecondOrderTurnRateFrac = 0.5f;

      //Mask = ComponentMask.LASER_PROJECTILE;
      CombatData.DamageType = DamageType.NORMAL;
    }
  }
}


