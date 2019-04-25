using SWEndor.Actors;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class GreenAntiShipLaserATI : Group.Projectile
  {
    internal GreenAntiShipLaserATI(Factory owner) : base(owner, "Green Anti-Ship Laser")
    {
      // Combat
      OnTimedLife = true;
      TimedLife = 5; // 6 seconds
      IsCombatObject = false;
      IsSelectable = false;
      IsDamage = true;
      ImpactDamage = 5;
      MaxSpeed = Globals.LaserSpeed;
      MinSpeed = Globals.LaserSpeed;

      NoAI = true;

      // Projectile
      ImpactCloseEnoughDistance = 100;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"projectiles\green3_laser.x");
    }

    public override void ProcessNewState(ActorInfo ainfo)
    {
      if (ainfo.ActorState == ActorState.DYING)
      {
        if (ainfo.CombatInfo.TimedLife > 0)
        {
          ActorCreationInfo acinfo = new ActorCreationInfo(Globals.Engine.ActorTypeFactory.Get("ExplosionSm"));
          acinfo.Position = ainfo.GetPosition();
          ActorInfo.Create(Owner.Engine.ActorFactory, acinfo);
        }
        ainfo.ActorState = ActorState.DEAD;
      }
    }
  }
}

