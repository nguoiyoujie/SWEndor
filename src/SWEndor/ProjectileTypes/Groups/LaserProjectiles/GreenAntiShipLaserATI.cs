using MTV3D65;
using SWEndor.ActorTypes.Components;
using SWEndor.Models;

namespace SWEndor.ProjectileTypes.Instances
{
  internal class GreenAntiShipLaserATI : Groups.LaserProjectile
  {
    internal GreenAntiShipLaserATI(Factory owner) : base(owner, "LSR_G3", "Green Anti-Ship Laser")
    {
      TimedLifeData = new TimedLifeData(true, 5);
      Explodes = new ExplodeData[] {
        new ExplodeData("EXPL00", 1, 1, ExplodeTrigger.ON_DEATH | ExplodeTrigger.ONLY_WHEN_DYINGTIME_NOT_EXPIRED)
      };

      CombatData.ImpactDamage = 5;
      CombatData.ImpactCloseEnoughDistance = 100;

      MeshData = new MeshData(Engine, Name, @"projectiles\green3_laser.x", 1, CONST_TV_BLENDINGMODE.TV_BLEND_ALPHA, "Laser");
    }
  }
}

