using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Data;
using SWEndor.ActorTypes.Components;
using SWEndor.Core;

namespace SWEndor.ActorTypes.Instances
{
  public class DeathStarLaserATI : Groups.LaserProjectile
  {
    internal DeathStarLaserATI(Factory owner) : base(owner, "Death Star Laser")
    {
      // Combat
      TimedLifeData = new TimedLifeData(true, 10);

      ImpactDamage = 99999;
      MoveLimitData.MaxSpeed = Globals.LaserSpeed * 85f;
      MoveLimitData.MinSpeed = Globals.LaserSpeed * 85f;

      IsLaser = false; // not the same speed

      RenderData.CullDistance = -1;

      MeshData = new MeshData(Name, @"projectiles\death_laser");
        
      ImpactCloseEnoughDistance = 200;
    }

    public override void ProcessState(Engine engine, ActorInfo ainfo)
    {
      // Override
    }

    public override void ProcessHit(Engine engine, ActorInfo owner, ActorInfo hitby, TV_3DVECTOR impact, TV_3DVECTOR normal)
    {
      base.ProcessHit(engine, owner, hitby, impact, normal);
      float time = 0.5f;

      if (hitby.DyingTimeRemaining > time)
        hitby.DyingTimerSet(time, false);
    }
  }
}

