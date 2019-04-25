using SWEndor.Actors;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class RedLaserATI : Group.Projectile
  {
    internal RedLaserATI(Factory owner) : base(owner, "Red Laser")
    {
      // Combat
      OnTimedLife = true;
      TimedLife = 1.6f; // 6 seconds
      IsCombatObject = false;
      IsSelectable = false;
      IsDamage = true;
      ImpactDamage = 1.0f;
      MaxSpeed = Globals.LaserSpeed;
      MinSpeed = Globals.LaserSpeed;

      NoAI = true;

      // Projectile
      ImpactCloseEnoughDistance = 25;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"projectiles\red_laser.x");
    }

    public override void ProcessNewState(ActorInfo ainfo)
    {
      if (ainfo.ActorState == ActorState.DYING)
      {
        if (ainfo.CombatInfo.TimedLife > 0)
        {
          ActorCreationInfo acinfo = new ActorCreationInfo(Globals.Engine.ActorTypeFactory.Get("Explosion"));
          acinfo.Position = ainfo.GetPosition();
          ActorInfo.Create(Owner.Engine.ActorFactory, acinfo);
        }
        ainfo.ActorState = ActorState.DEAD;
      }
    }
  }
}

