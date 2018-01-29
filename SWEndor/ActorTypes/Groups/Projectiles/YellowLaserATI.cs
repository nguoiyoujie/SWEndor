using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class YellowLaserATI : ProjectileGroup
  {
    private static YellowLaserATI _instance;
    public static YellowLaserATI Instance()
    {
      if (_instance == null) { _instance = new YellowLaserATI(); }
      return _instance;
    }

    private YellowLaserATI() : base("Yellow Laser")
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

      // Projectile
      ImpactCloseEnoughDistance = 35;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"projectiles\yellow_laser.x");
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

