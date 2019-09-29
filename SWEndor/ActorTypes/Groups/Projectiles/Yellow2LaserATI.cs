using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class Yellow2LaserATI : Groups.LaserProjectile
  {
    internal Yellow2LaserATI(Factory owner) : base(owner, "Yellow Double Laser")
    {
      ImpactDamage = 1.5f;
      ImpactCloseEnoughDistance = 50;

      MeshData = new MeshData(Name, @"projectiles\yellow2_laser.x");
    }
  }
}

