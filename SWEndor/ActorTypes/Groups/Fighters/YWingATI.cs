using MTV3D65;
using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class YWingATI : Groups.RebelWing
  {
    internal YWingATI(Factory owner) : base(owner, "Y-Wing")
    {
      MaxStrength = 36;
      ImpactDamage = 16;
      MoveLimitData.MaxSpeed = 300;
      MoveLimitData.MinSpeed = 150;
      MoveLimitData.MaxSpeedChangeRate = 150;
      MoveLimitData.MaxTurnRate = 32;

      ScoreData = new ScoreData(400, 2000);

      RegenData = new RegenData(false, 0.08f, 0, 0, 0);

      MeshData = new MeshData(Name, @"ywing\ywing.x");

      Cameras = new LookData[] {
        new LookData(new TV_3DVECTOR(0, 4, 30), new TV_3DVECTOR(0, 0, 2000)),
        new LookData(new TV_3DVECTOR(0, 25, -100), new TV_3DVECTOR(0, 0, 2000)),
        new LookData(new TV_3DVECTOR(0, 0, -40), new TV_3DVECTOR(0, 0, -2000))
      };

      Debris = new DebrisSpawnerData[] {
        new DebrisSpawnerData("YWing_WingATI", new TV_3DVECTOR(-30, 0, 0), -10, 10, 0, 30, -25, 25, 0.5f),
        new DebrisSpawnerData("YWing_WingATI", new TV_3DVECTOR(30, 0, 0), -10, 10, -30, 0, -25, 25, 0.5f),
        };

      Loadouts = new string[] { "Y_WG_TORP", "Y_WG_ION", "Y_WG_LASR" };
    }
  }
}

