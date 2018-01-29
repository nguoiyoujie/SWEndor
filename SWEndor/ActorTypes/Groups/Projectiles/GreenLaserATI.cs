using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class GreenLaserATI : ProjectileGroup
  {
    private static GreenLaserATI _instance;
    public static GreenLaserATI Instance()
    {
      if (_instance == null) { _instance = new GreenLaserATI(); }
      return _instance;
    }

    private GreenLaserATI() : base("Green Laser")
    {
      // Combat
      OnTimedLife = true;
      TimedLife = 1.6f; // 6 seconds
      IsCombatObject = false;
      IsSelectable = false;
      IsDamage = true;
      ImpactDamage = 1;
      MaxSpeed = Globals.LaserSpeed;
      MinSpeed = Globals.LaserSpeed;

      NoAI = true;

      // Projectile
      ImpactCloseEnoughDistance = 30;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"projectiles\green_laser.x");
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

