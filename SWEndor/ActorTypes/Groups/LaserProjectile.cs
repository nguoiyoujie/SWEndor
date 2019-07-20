using SWEndor.Actors;
using SWEndor.Actors.Data;
using SWEndor.Actors.Traits;

namespace SWEndor.ActorTypes.Groups
{
  public class LaserProjectile : Projectile
  {
    internal LaserProjectile(Factory owner, string name) : base(owner, name)
    {
      // Combat
      TimedLifeData = new TimedLifeData(true, 1.6f);
      CombatData = CombatData.Disabled;

      Explodes = new ExplodeInfo[] {
        new ExplodeInfo("ExpS00", 1, 1, ExplodeTrigger.ON_DEATH | ExplodeTrigger.ONLY_WHEN_DYINGTIME_NOT_EXPIRED)
      };

      MaxSpeed = Globals.LaserSpeed;
      MinSpeed = Globals.LaserSpeed;

      IsLaser = true;
      Mask = ComponentMask.LASER_PROJECTILE;
    }
  }
}


