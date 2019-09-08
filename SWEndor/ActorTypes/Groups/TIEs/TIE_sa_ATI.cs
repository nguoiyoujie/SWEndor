using MTV3D65;
using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class TIE_sa_ATI : Groups.TIE
  {
    internal TIE_sa_ATI(Factory owner) : base(owner, "TIE Bomber")
    {
      MaxStrength = 6;
      ImpactDamage = 16;
      MaxSpeed = 300;
      MinSpeed = 150;
      MaxSpeedChangeRate = 200;
      MaxTurnRate = 40;

      Score_perStrength = 350;
      Score_DestroyBonus = 500;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"tie_vader\tie_bomber.x");

      Cameras = new ActorCameraInfo[]
      {
        new ActorCameraInfo(new TV_3DVECTOR(0, 0, 20), new TV_3DVECTOR(0, 0, 2000)),
        new ActorCameraInfo(new TV_3DVECTOR(0, 25, -100), new TV_3DVECTOR(0, 0, 2000)),
        new ActorCameraInfo(new TV_3DVECTOR(0, 0, -40), new TV_3DVECTOR(0, 0, -2000))
      };

      Loadouts = new string[] { "TIEB_TORP", "TIEB_ION", "TIEB_LASR" };
    }
  }
}

