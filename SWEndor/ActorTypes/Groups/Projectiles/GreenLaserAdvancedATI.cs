using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class GreenLaserAdvancedATI : Groups.LaserProjectile
  {
    internal GreenLaserAdvancedATI(Factory owner) : base(owner, "Green Laser Advanced")
    {
      ImpactDamage = 1.75f;
      ImpactCloseEnoughDistance = 60;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"projectiles\green_laser.x");
    }
  }
}

