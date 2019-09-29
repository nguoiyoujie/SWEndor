using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class RedLaserATI : Groups.LaserProjectile
  {
    internal RedLaserATI(Factory owner) : base(owner, "Red Laser")
    {
      ImpactDamage = 1;
      ImpactCloseEnoughDistance = 25;

      MeshData = new MeshData(Name, @"projectiles\red_laser.x");
    }
  }
}

