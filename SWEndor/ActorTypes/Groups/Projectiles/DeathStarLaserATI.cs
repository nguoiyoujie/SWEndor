using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Actors.Data;

namespace SWEndor.ActorTypes.Instances
{
  public class DeathStarLaserATI : Groups.LaserProjectile
  {
    internal DeathStarLaserATI(Factory owner) : base(owner, "Death Star Laser")
    {
      // Combat
      TimedLifeData = new TimedLifeData(true, 10);

      ImpactDamage = 99999;
      MaxSpeed = Globals.LaserSpeed * 85f;
      MinSpeed = Globals.LaserSpeed * 85f;

      IsLaser = false; // not the same speed

      EnableDistanceCull = false;

      SourceMesh = TrueVision.TVGlobals.GetMesh(Name);
      if (SourceMesh == null)
      {
        SourceMesh = TrueVision.TVScene.CreateMeshBuilder(Name);

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

    public override void ProcessHit(ActorInfo owner, ActorInfo hitby, TV_3DVECTOR impact, TV_3DVECTOR normal)
    {
      base.ProcessHit(owner, hitby, impact, normal);

      TimedLifeSystem.ReduceTimerTo(Engine, hitby, 0.5f);
      TimedLifeSystem.ReduceTimerTo(Engine, owner, 0);
    }
  }
}

