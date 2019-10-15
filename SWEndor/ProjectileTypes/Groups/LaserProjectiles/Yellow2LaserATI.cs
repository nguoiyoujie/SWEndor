using MTV3D65;
using SWEndor.ActorTypes.Components;

namespace SWEndor.ProjectileTypes.Instances
{
  public class Yellow2LaserATI : Groups.LaserProjectile
  {
    internal Yellow2LaserATI(Factory owner) : base(owner, "LSR_Y2", "Yellow Double Laser")
    {
      CombatData.ImpactDamage = 1.5f;
      CombatData.ImpactCloseEnoughDistance = 50;

      MeshData = new MeshData(Name, @"projectiles\yellow2_laser.x", 1, CONST_TV_BLENDINGMODE.TV_BLEND_ALPHA, "Laser");
    }
  }
}

