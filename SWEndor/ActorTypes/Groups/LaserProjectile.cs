using SWEndor.Actors.Data;
using SWEndor.ActorTypes.Components;
using SWEndor.Models;

namespace SWEndor.ActorTypes.Groups
{
  public class LaserProjectile : Projectile
  {
    internal LaserProjectile(Factory owner, string name) : base(owner, name)
    {
      // Combat
      TimedLifeData = new TimedLifeData(true, 1.6f);
      CombatData = CombatData.Disabled;
      ArmorData = ArmorData.Immune;
      Explodes = new ExplodeData[] {
        new ExplodeData("ExpS00", 1, 1, ExplodeTrigger.ON_DEATH | ExplodeTrigger.ONLY_WHEN_DYINGTIME_NOT_EXPIRED)
      };

      MoveLimitData.MaxSpeed = Globals.LaserSpeed;
      MoveLimitData.MinSpeed = Globals.LaserSpeed;

      IsLaser = true;
      Mask = ComponentMask.LASER_PROJECTILE;
    }
  }
}


