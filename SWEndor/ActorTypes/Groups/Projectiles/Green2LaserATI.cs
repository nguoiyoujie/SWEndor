using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class Green2LaserATI : Groups.LaserProjectile
  {
    internal Green2LaserATI(Factory owner) : base(owner, "LSR_G2", "Green Double Laser")
    {
      ImpactDamage = 2;
      AIData.ImpactCloseEnoughDistance = 35;

      MeshData = new MeshData(Name, @"projectiles\green2_laser.x");
    }
  }
}

