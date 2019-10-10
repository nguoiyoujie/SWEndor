using SWEndor.ActorTypes;
using SWEndor.ActorTypes.Components;
using SWEndor.Models;

namespace SWEndor.ProjectileTypes.Groups
{
  public class LaserProjectile : Projectile
  {
    internal LaserProjectile(Factory owner, string id, string name) : base(owner, id, name)
    {
      // Combat
      TimedLifeData = new TimedLifeData(true, 2.5f);
      CombatData = CombatData.Disabled;
      Explodes = new ExplodeData[] {
        new ExplodeData("EXPS00", 1, 1, ExplodeTrigger.ON_DEATH | ExplodeTrigger.ONLY_WHEN_DYINGTIME_NOT_EXPIRED)
      };

      MoveLimitData.MaxSpeed = Globals.LaserSpeed;
      MoveLimitData.MinSpeed = Globals.LaserSpeed;

      CombatData.IsLaser = true;
      Mask = ComponentMask.LASER_PROJECTILE;
    }
  }
}


