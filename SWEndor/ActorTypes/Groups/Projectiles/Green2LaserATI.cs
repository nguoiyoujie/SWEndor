using SWEndor.Actors;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Green2LaserATI : Group.Projectile
  {
    internal Green2LaserATI(Factory owner) : base(owner, "Green Double Laser")
    {
      // Combat
      OnTimedLife = true;
      TimedLife = 1.6f; // 6 seconds
      IsCombatObject = false;
      IsSelectable = false;
      IsDamage = true;
      ImpactDamage = 2;
      MaxSpeed = Globals.LaserSpeed;
      MinSpeed = Globals.LaserSpeed;

      NoAI = true;

      // Projectile
      ImpactCloseEnoughDistance = 35;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"projectiles\green2_laser.x");
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

