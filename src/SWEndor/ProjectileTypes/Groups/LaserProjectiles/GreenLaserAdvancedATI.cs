using MTV3D65;
using SWEndor.ActorTypes.Components;

namespace SWEndor.ProjectileTypes.Instances
{
  public class GreenLaserAdvancedATI : Groups.LaserProjectile
  {
    internal GreenLaserAdvancedATI(Factory owner) : base(owner, "LSR_GADV", "Green Laser Advanced")
    {
      CombatData.ImpactDamage = 1.75f;
      CombatData.ImpactCloseEnoughDistance = 60;

      MeshData = new MeshData(Name, @"projectiles\green_laser.x", 1, CONST_TV_BLENDINGMODE.TV_BLEND_ALPHA, "Laser");
    }
  }
}

