using SWEndor.Actors;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Yellow2LaserATI : Group.Projectile
  {
    internal Yellow2LaserATI(Factory owner) : base(owner, "Yellow Double Laser")
    {
      // Combat
      OnTimedLife = true;
      TimedLife = 1.6f; // 6 seconds
      IsCombatObject = false;
      IsSelectable = false;
      IsDamage = true;
      ImpactDamage = 1.5f;
      MaxSpeed = Globals.LaserSpeed;
      MinSpeed = Globals.LaserSpeed;

      NoAI = true;

      // Projectile
      ImpactCloseEnoughDistance = 50;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"projectiles\yellow2_laser.x");
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

