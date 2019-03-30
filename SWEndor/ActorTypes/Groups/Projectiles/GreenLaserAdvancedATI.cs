using System.IO;

namespace SWEndor.Actors.Types
{
  public class GreenLaserAdvancedATI : ProjectileGroup
  {
    private static GreenLaserAdvancedATI _instance;
    public static GreenLaserAdvancedATI Instance()
    {
      if (_instance == null) { _instance = new GreenLaserAdvancedATI(); }
      return _instance;
    }

    private GreenLaserAdvancedATI() : base("Green Laser Advanced")
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
          ActorCreationInfo acinfo = new ActorCreationInfo(ActorTypeFactory.Instance().GetActorType("Explosion"));
          acinfo.Position = ainfo.GetPosition();
          ActorInfo.Create(acinfo);
        }
        ainfo.ActorState = ActorState.DEAD;
      }
    }
  }
}

