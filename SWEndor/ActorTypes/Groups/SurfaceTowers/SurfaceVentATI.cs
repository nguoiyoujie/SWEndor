using MTV3D65;
using System.IO;

namespace SWEndor.Actors.Types
{
  public class SurfaceVentATI : SurfaceTowerGroup
  {
    private static SurfaceVentATI _instance;
    public static SurfaceVentATI Instance()
    {
      if (_instance == null) { _instance = new SurfaceVentATI(); }
      return _instance;
    }

    private SurfaceVentATI() : base("Thermal Exhaust Port")
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

    public override void ProcessHit(ActorInfo ainfo, ActorInfo hitby, TV_3DVECTOR impact, TV_3DVECTOR normal)
    {
      base.ProcessHit(ainfo, hitby, impact, normal);

      if (hitby.TypeInfo is TorpedoATI) //hard code?
      {
        ainfo.ActorState = ActorState.DEAD;
      }
    }
  }
}

