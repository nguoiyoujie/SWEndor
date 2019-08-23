using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class RedLaserATI : Groups.LaserProjectile
  {
    internal RedLaserATI(Factory owner) : base(owner, "Red Laser")
    {
      ImpactDamage = 1;
      ImpactCloseEnoughDistance = 25;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"projectiles\red_laser.x");
    }
  }
}

