using MTV3D65;
using SWEndor.Actors;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class SurfaceVentATI : Groups.SurfaceTower
  {
    internal SurfaceVentATI(Factory owner) : base(owner, "Thermal Exhaust Port")
    {
      MaxStrength = 12000;
      ImpactDamage = 120;

      RadarSize = 2.5f;

      Score_perStrength = 50;
      Score_DestroyBonus = 5000;

      NoMove = true;
      NoAI = true;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"surface\surface_vent.x");
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      // Tower Gun
      ainfo.ExplosionInfo.EnableDeathExplosion = true;
      ainfo.ExplosionInfo.DeathExplosionType = "ExplosionSm";
      ainfo.ExplosionInfo.DeathExplosionSize = 5;
    }

    public override void ProcessHit(int ownerActorID, int hitbyActorID, TV_3DVECTOR impact, TV_3DVECTOR normal)
    {
      base.ProcessHit(ownerActorID, hitbyActorID, impact, normal);
      ActorInfo owner = Owner.Engine.ActorFactory.Get(ownerActorID);
      ActorInfo hitby = Owner.Engine.ActorFactory.Get(hitbyActorID);

      if (owner == null || hitby == null)
        return;

      if (hitby.TypeInfo is TorpedoATI) //hard code?
        owner.ActorState = ActorState.DEAD;
    }
  }
}

