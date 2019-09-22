using MTV3D65;
using SWEndor.ActorTypes.Components;
using SWEndor.Sound;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class BWingATI : Groups.RebelWing
  {
    internal BWingATI(Factory owner) : base(owner, "B-Wing")
    {
      MaxStrength = 45;
      ImpactDamage = 16;
      MoveLimitData.MaxSpeed = 400;
      MoveLimitData.MinSpeed = 200;
      MoveLimitData.MaxSpeedChangeRate = 200;
      MoveLimitData.MaxTurnRate = 40;

      ScoreData = new ScoreData(400, 2500);

      RegenData = new RegenData(false, 0.08f, 0, 0, 0);

      MeshData = new MeshData(Name, @"bwing\bwing.x");

      Cameras = new LookData[] {
        new LookData(new TV_3DVECTOR(0, 0, 14), new TV_3DVECTOR(0, 0, 2000)),
        new LookData(new TV_3DVECTOR(0, 25, -100), new TV_3DVECTOR(0, 0, 2000)),
        new LookData(new TV_3DVECTOR(0, 0, -40), new TV_3DVECTOR(0, 0, -2000))
     };

      Debris = new DebrisSpawnerData[] {
        new DebrisSpawnerData("BWing_WingATI", new TV_3DVECTOR(-30, -30, 0), -10, 10, 0, 30, -25, 25, 0.5f),
        new DebrisSpawnerData("BWing_WingATI", new TV_3DVECTOR(30, -30, 0), -10, 10, -30, 0, -25, 25, 0.5f),
        new DebrisSpawnerData("BWing_Top_WingATI", new TV_3DVECTOR(0, 0, 0), -10, 10, -10, 10, -25, 25, 0.5f),
        new DebrisSpawnerData("BWing_Bottom_WingATI", new TV_3DVECTOR(0, -70, 0), -10, 10, -10, 10, -25, 25, 0.5f)
        };

      SoundSources = new SoundSourceData[] { new SoundSourceData(SoundGlobals.EngineXWing, 200f, new TV_3DVECTOR(0, 0, -30), true) };

      Loadouts = new string[] { "B_WG_TORP", "B_WG_ION", "B_WG_LASR" };
    }
  }
}

