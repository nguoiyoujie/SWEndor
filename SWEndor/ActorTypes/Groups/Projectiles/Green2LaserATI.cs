using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class Green2LaserATI : ProjectileGroup
  {
    private static Green2LaserATI _instance;
    public static Green2LaserATI Instance()
    {
      if (_instance == null) { _instance = new Green2LaserATI(); }
      return _instance;
    }

    private Green2LaserATI() : base("Green Double Laser")
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
      ImpactCloseEnoughDistance = 30;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"projectiles\green2_laser.x");
    }

    public override void ProcessNewState(ActorInfo ainfo)
    {
      if (ainfo.ActorState == ActorState.DYING)
      {
        if (ainfo.TimedLife > 0)
        {
          ActorCreationInfo acinfo = new ActorCreationInfo(ActorTypeFactory.Instance().GetActorType("Explosion"));
          acinfo.Position = ainfo.GetPosition();
          ActorInfo.Create(acinfo);
        }
        ainfo.ActorState = ActorState.DEAD;
      }
    }
  }
}

