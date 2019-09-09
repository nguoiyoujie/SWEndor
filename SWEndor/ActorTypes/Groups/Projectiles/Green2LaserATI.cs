using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Green2LaserATI : Groups.LaserProjectile
  {
    internal Green2LaserATI(Factory owner) : base(owner, "Green Double Laser")
    {
      ImpactDamage = 2;
      ImpactCloseEnoughDistance = 35;

      MeshData = new MeshData(Name, @"projectiles\green2_laser.x");
    }
  }
}

