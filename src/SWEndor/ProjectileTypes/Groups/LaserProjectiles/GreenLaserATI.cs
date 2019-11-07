using MTV3D65;
using SWEndor.ActorTypes.Components;

namespace SWEndor.ProjectileTypes.Instances
{
  public class GreenLaserATI : Groups.LaserProjectile
  {
    internal GreenLaserATI(Factory owner) : base(owner, "LSR_G", "Green Laser")
    {
      CombatData.ImpactDamage = 1;
      CombatData.ImpactCloseEnoughDistance = 35;

      MeshData = new MeshData(Name, @"projectiles\green_laser.x", 1, CONST_TV_BLENDINGMODE.TV_BLEND_ALPHA, "Laser");
    }
  }
}

