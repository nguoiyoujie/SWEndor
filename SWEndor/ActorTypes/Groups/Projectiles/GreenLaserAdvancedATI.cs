using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class GreenLaserAdvancedATI : Groups.LaserProjectile
  {
    internal GreenLaserAdvancedATI(Factory owner) : base(owner, "LSR_GADV", "Green Laser Advanced")
    {
      ImpactDamage = 1.75f;
      AIData.ImpactCloseEnoughDistance = 60;

      MeshData = new MeshData(Name, @"projectiles\green_laser.x");
    }
  }
}

