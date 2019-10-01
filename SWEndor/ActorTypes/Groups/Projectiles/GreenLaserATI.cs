using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class GreenLaserATI : Groups.LaserProjectile
  {
    internal GreenLaserATI(Factory owner) : base(owner, "LSR_G", "Green Laser")
    {
      ImpactDamage = 1;
      ImpactCloseEnoughDistance = 35;

      MeshData = new MeshData(Name, @"projectiles\green_laser.x");
    }
  }
}

