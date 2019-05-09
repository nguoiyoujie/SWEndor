using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class GreenLaserATI : Groups.LaserProjectile
  {
    internal GreenLaserATI(Factory owner) : base(owner, "Green Laser")
    {
      ImpactDamage = 1;
      ImpactCloseEnoughDistance = 35;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"projectiles\green_laser.x");
    }
  }
}

