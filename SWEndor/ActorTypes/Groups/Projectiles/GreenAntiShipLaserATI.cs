using SWEndor.Actors.Data;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class GreenAntiShipLaserATI : Groups.LaserProjectile
  {
    internal GreenAntiShipLaserATI(Factory owner) : base(owner, "Green Anti-Ship Laser")
    {
      TimedLifeData = new TimedLifeData(true, 5);
      ExplodeData = new ExplodeData(deathTrigger: DeathExplosionTrigger.TIMENOTEXPIRED_ONLY, deathExplosionType: "ExplosionSm");

      ImpactDamage = 5;
      ImpactCloseEnoughDistance = 100;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"projectiles\green3_laser.x");
    }
  }
}

