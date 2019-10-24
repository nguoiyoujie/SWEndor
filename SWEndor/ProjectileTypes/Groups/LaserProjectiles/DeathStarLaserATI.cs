using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes.Components;
using SWEndor.Core;
using SWEndor.Projectiles;

namespace SWEndor.ProjectileTypes.Instances
{
  public class DeathStarLaserATI : Groups.LaserProjectile
  {
    internal DeathStarLaserATI(Factory owner) : base(owner, "LSR_DS", "Death Star Laser")
    {
      // Combat
      TimedLifeData = new TimedLifeData(true, 10);

      CombatData.ImpactDamage = 99999;
      MoveLimitData.MaxSpeed = Globals.LaserSpeed * 5f;
      MoveLimitData.MinSpeed = Globals.LaserSpeed * 5f;

      CombatData.IsLaser = false; // not the same speed

      RenderData.CullDistance = -1;

      MeshData = new MeshData(Name, @"projectiles\death_laser.x");

      CombatData.ImpactCloseEnoughDistance = 200;
    }

    public override void ProcessState(Engine engine, ProjectileInfo ainfo)
    {
      // Override
    }

    public override void ProcessHit(Engine engine, ProjectileInfo owner, ActorInfo hitby, TV_3DVECTOR impact)
    {
      base.ProcessHit(engine, owner, hitby, impact);
      float time = 0.5f;

      if (hitby.DyingTimeRemaining > time)
        hitby.DyingTimerSet(time, false);
    }
  }
}

