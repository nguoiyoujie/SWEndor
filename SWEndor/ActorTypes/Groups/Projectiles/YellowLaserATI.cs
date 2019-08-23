using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class YellowLaserATI : Groups.LaserProjectile
  {
    internal YellowLaserATI(Factory owner) : base(owner, "Yellow Laser")
    {
      ImpactDamage = 1;
      MaxSpeed = Globals.LaserSpeed * 0.75f;
      MinSpeed = Globals.LaserSpeed * 0.75f;

      ImpactCloseEnoughDistance = 35;
      IsLaser = false; // not the same speed

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"projectiles\yellow_laser.x");
    }
  }
}

