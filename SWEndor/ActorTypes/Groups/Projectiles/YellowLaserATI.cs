using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class YellowLaserATI : Groups.LaserProjectile
  {
    internal YellowLaserATI(Factory owner) : base(owner, "Yellow Laser")
    {
      ImpactDamage = 1;
      MoveLimitData.MaxSpeed = Globals.LaserSpeed * 0.75f;
      MoveLimitData.MinSpeed = Globals.LaserSpeed * 0.75f;

      ImpactCloseEnoughDistance = 35;
      IsLaser = false; // not the same speed

      MeshData = new MeshData(Name, @"projectiles\yellow_laser.x");
    }
  }
}

