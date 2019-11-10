using MTV3D65;
using SWEndor.ActorTypes.Components;

namespace SWEndor.ProjectileTypes.Instances
{
  internal class YellowLaserATI : Groups.LaserProjectile
  {
    internal YellowLaserATI(Factory owner) : base(owner, "LSR_Y", "Yellow Laser")
    {
      CombatData.ImpactDamage = 1;
      MoveLimitData.MaxSpeed = Globals.LaserSpeed * 0.75f;
      MoveLimitData.MinSpeed = Globals.LaserSpeed * 0.75f;

      CombatData.ImpactCloseEnoughDistance = 35;
      CombatData.IsLaser = false; // not the same speed

      MeshData = new MeshData(Name, @"projectiles\yellow_laser.x", 1, CONST_TV_BLENDINGMODE.TV_BLEND_ALPHA, "Laser");
    }
  }
}

