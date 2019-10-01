using SWEndor.ActorTypes.Components;
using SWEndor.Models;

namespace SWEndor.ActorTypes.Instances
{
  public class GreenAntiShipLaserATI : Groups.LaserProjectile
  {
    internal GreenAntiShipLaserATI(Factory owner) : base(owner, "LSR_G3", "Green Anti-Ship Laser")
    {
      TimedLifeData = new TimedLifeData(true, 5);
      Explodes = new ExplodeData[] {
        new ExplodeData("EXPL00", 1, 1, ExplodeTrigger.ON_DEATH | ExplodeTrigger.ONLY_WHEN_DYINGTIME_NOT_EXPIRED)
      };

      ImpactDamage = 5;
      AIData.ImpactCloseEnoughDistance = 100;

      MeshData = new MeshData(Name, @"projectiles\green3_laser.x");
    }
  }
}

