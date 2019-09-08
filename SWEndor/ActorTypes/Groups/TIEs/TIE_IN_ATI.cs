using MTV3D65;
using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class TIE_IN_ATI : Groups.TIE
  {
    internal TIE_IN_ATI(Factory owner) : base(owner, "TIE Interceptor")
    {
      MaxStrength = 8;
      ImpactDamage = 16;
      MaxSpeed = 500;
      MinSpeed = 250;
      MaxSpeedChangeRate = 200;
      MaxTurnRate = 55;

      Score_perStrength = 500;
      Score_DestroyBonus = 1000;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"tie\tie_interceptor.x");
      SourceFarMeshPath = Path.Combine(Globals.ModelPath, @"tie\tie_interceptor_far.x");

      Cameras = new ActorCameraInfo[]
      {
        new ActorCameraInfo(new TV_3DVECTOR(0, 0, 20), new TV_3DVECTOR(0, 0, 2000)),
        new ActorCameraInfo(new TV_3DVECTOR(0, 25, -100), new TV_3DVECTOR(0, 0, 2000)),
        new ActorCameraInfo(new TV_3DVECTOR(0, 0, -40), new TV_3DVECTOR(0, 0, -2000))
      };

      Debris = new DebrisSpawnerInfo[] {
        new DebrisSpawnerInfo("TIE_InterceptorWingATI", new TV_3DVECTOR(-30, 0, 0), -1000, 1000, 0, 3000, -2500, 2500, 0.5f),
        new DebrisSpawnerInfo("TIE_InterceptorWingATI", new TV_3DVECTOR(30, 0, 0), -1000, 1000, -3000, 0, -2500, 2500, 0.5f),
        };

      Loadouts = new string[] { "TIEI_LASR", "TIEI_LASR_AI" };
    }
  }
}

