using MTV3D65;
using SWEndor.Actors;

namespace SWEndor.ActorTypes.Instances
{
  public class DeathStarLaserATI : Group.Projectile
  {
    internal DeathStarLaserATI(Factory owner) : base(owner, "Death Star Laser")
    {
      // Combat
      OnTimedLife = true;
      TimedLife = 10;
      IsCombatObject = false;
      IsSelectable = false;
      IsDamage = true;
      ImpactDamage = 99999;
      MaxSpeed = Globals.LaserSpeed * 8.5f;
      MinSpeed = Globals.LaserSpeed * 8.5f;

      NoAI = true;
      IsLaser = true;
      EnableDistanceCull = false;

      SourceMesh = TrueVision.TVGlobals.GetMesh(Key);
      if (SourceMesh == null)
      {
        SourceMesh = TrueVision.TVScene.CreateMeshBuilder(Key);

        SourceMesh.CreateBox(40, 40, 1000);
        SourceMesh.SetMeshCenter(0, 0, 2200);
        SourceMesh.SetColor(new TV_COLOR(0, 1, 0, 1).GetIntColor());

        SourceMesh.Enable(false);

        SourceMesh.SetCollisionEnable(false);

        ImpactCloseEnoughDistance = 200;
      }
    }

    public override void ProcessState(ActorInfo ainfo)
    {
      // Override
    }

    public override void ProcessHit(int ownerActorID, int hitbyActorID, TV_3DVECTOR impact, TV_3DVECTOR normal)
    {
      base.ProcessHit(ownerActorID, hitbyActorID, impact, normal);
      ActorInfo owner = ActorFactory.Get(ownerActorID);
      ActorInfo hitby = ActorFactory.Get(hitbyActorID);

      if (owner == null || hitby == null)
        return;

      if (hitby.CombatInfo.TimedLife > 0.5f)
        hitby.CombatInfo.TimedLife = 0.5f;

      if (owner.CombatInfo.TimedLife > 0)
        owner.CombatInfo.TimedLife = 0;
    }
  }
}

