using MTV3D65;
using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class TIE_D_ATI : Groups.TIE
  {
    internal TIE_D_ATI(Factory owner) : base(owner, "TIE Defender")
    {
      MaxStrength = 24;
      ImpactDamage = 16;
      MoveLimitData.MaxSpeed = 600;
      MoveLimitData.MinSpeed = 300;
      MoveLimitData.MaxSpeedChangeRate = 300;
      MoveLimitData.MaxTurnRate = 65;

      MoveLimitData.ZTilt = 2.75f;
      MoveLimitData.ZNormFrac = 0.005f;

      ScoreData = new ScoreData(800, 2000);

      RegenData = new RegenData(false, 0.075f, 0, 0, 0);

      MeshData = new MeshData(Name, @"tie\tie_defender.x");

      Cameras = new LookData[]
      {
        new LookData(new TV_3DVECTOR(0, 0, 6), new TV_3DVECTOR(0, 0, 2000)),
        new LookData(new TV_3DVECTOR(0, 25, -100), new TV_3DVECTOR(0, 0, 2000)),
        new LookData(new TV_3DVECTOR(0, 0, -40), new TV_3DVECTOR(0, 0, -2000))
      };

      Debris = new DebrisSpawnerData[] {
        new DebrisSpawnerData("TIE_InterceptorWingATI", new TV_3DVECTOR(-30, 0, 0), -10, 10, 0, 30, -25, 25, 0.5f),
        new DebrisSpawnerData("TIE_InterceptorWingATI", new TV_3DVECTOR(30, 0, 0), -10, 10, -30, 0, -25, 25, 0.5f),
        };

      Loadouts = new string[] { "TIED_LASR" };
    }
  }
}

