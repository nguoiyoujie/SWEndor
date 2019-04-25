using SWEndor.Actors;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class GreenLaserAdvancedATI : Group.Projectile
  {
    internal GreenLaserAdvancedATI(Factory owner) : base(owner, "Green Laser Advanced")
    {
      // Combat
      OnTimedLife = true;
      TimedLife = 1.6f; // 6 seconds
      IsCombatObject = false;
      IsSelectable = false;
      IsDamage = true;
      ImpactDamage = 1.75f;
      MaxSpeed = Globals.LaserSpeed;
      MinSpeed = Globals.LaserSpeed;

      NoAI = true;

      // Projectile
      ImpactCloseEnoughDistance = 60;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"projectiles\green_laser.x");
      if (SourceMesh == null)
      {
        /*
        CollisionRays.Add(new TV_3DVectorRay(new TV_3DVECTOR(0, 0, 35), new TV_3DVECTOR(30, 30, -200)));
        CollisionRays.Add(new TV_3DVectorRay(new TV_3DVECTOR(0, 0, 35), new TV_3DVECTOR(-30, 30, -200)));
        CollisionRays.Add(new TV_3DVectorRay(new TV_3DVECTOR(0, 0, 35), new TV_3DVECTOR(30, -30, -200)));
        CollisionRays.Add(new TV_3DVectorRay(new TV_3DVECTOR(0, 0, 35), new TV_3DVECTOR(-30, -30, -200)));
        */
      }
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

