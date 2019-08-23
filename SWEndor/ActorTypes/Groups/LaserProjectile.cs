using SWEndor.Actors;
using SWEndor.Actors.Data;
using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Groups
{
  public class LaserProjectile : Projectile
  {
    internal LaserProjectile(Factory owner, string name) : base(owner, name)
    {
      // Combat
      TimedLifeData = new TimedLifeData(true, 1.6f);
      CombatData = CombatData.Disabled;
      Armor = ArmorInfo.Immune;
      ExplodeData = new ExplodeData(deathTrigger: DeathExplosionTrigger.TIMENOTEXPIRED_ONLY, deathExplosionType: "Explosion");

      MaxSpeed = Globals.LaserSpeed;
      MinSpeed = Globals.LaserSpeed;

      IsLaser = true;
      Mask = ComponentMask.LASER_PROJECTILE;
    }
  }
}


