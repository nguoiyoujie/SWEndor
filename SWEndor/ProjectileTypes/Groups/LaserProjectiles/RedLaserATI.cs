using MTV3D65;
using SWEndor.ActorTypes.Components;

namespace SWEndor.ProjectileTypes.Instances
{
  public class RedLaserATI : Groups.LaserProjectile
  {
    internal RedLaserATI(Factory owner) : base(owner, "LSR_R", "Red Laser")
    {
      CombatData.ImpactDamage = 1;
      CombatData.ImpactCloseEnoughDistance = 25;

      MeshData = new MeshData(Name, @"projectiles\red_laser.x", 1, CONST_TV_BLENDINGMODE.TV_BLEND_ALPHA, "Laser");
    }
  }
}

