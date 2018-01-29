using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class Yellow2LaserATI : ProjectileGroup
  {
    private static Yellow2LaserATI _instance;
    public static Yellow2LaserATI Instance()
    {
      if (_instance == null) { _instance = new Yellow2LaserATI(); }
      return _instance;
    }

    private Yellow2LaserATI() : base("Yellow Double Laser")
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
      ImpactCloseEnoughDistance = 25;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"projectiles\yellow2_laser.x");
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

