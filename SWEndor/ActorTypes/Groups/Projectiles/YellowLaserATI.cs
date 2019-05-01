using SWEndor.Actors;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class YellowLaserATI : Group.Projectile
  {
    internal YellowLaserATI(Factory owner) : base(owner, "Yellow Laser")
    {
      // Combat
      OnTimedLife = true;
      TimedLife = 1.6f; // 6 seconds
      IsCombatObject = false;
      IsSelectable = false;
      IsDamage = true;
      ImpactDamage = 1;
      MaxSpeed = Globals.LaserSpeed * 0.75f;
      MinSpeed = Globals.LaserSpeed * 0.75f;

      NoAI = true;
      IsLaser = true;

      // Projectile
      ImpactCloseEnoughDistance = 35;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"projectiles\yellow_laser.x");
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.ExplosionInfo.DeathExplosionType = "Explosion";
    }
  }
}

