using MTV3D65;
using SWEndor.Actors.Models;
using SWEndor.ActorTypes.Components;
using SWEndor.Sound;

namespace SWEndor.ActorTypes.Instances
{
  internal class Z95ATI : Groups.RebelWing
  {
    internal Z95ATI(Factory owner) : base(owner, "Z95", "Z-95")
    {
      SystemData.MaxShield = 12;
      SystemData.MaxHull = 4;
      CombatData.ImpactDamage = 16;
      MoveLimitData.MaxSpeed = 400;
      MoveLimitData.MinSpeed = 175;
      MoveLimitData.MaxSpeedChangeRate = 200;
      MoveLimitData.MaxTurnRate = 42;

      ScoreData = new ScoreData(400, 2000);
      RegenData = new RegenData(false, 0.12f, 0, 0, 0);
      MeshData = new MeshData(Name, @"xwing\z95.x");
      SoundSources = new SoundSourceData[] { new SoundSourceData(SoundGlobals.EngineXWing, 200f, new TV_3DVECTOR(0, 0, -30), true, isEngineSound: true) };

      Cameras = new LookData[] {
        new LookData(new TV_3DVECTOR(0, 3, 25), new TV_3DVECTOR(0, 3, 2000)),
        new LookData(new TV_3DVECTOR(0, 25, -100), new TV_3DVECTOR(0, 0, 2000)),
        new LookData(new TV_3DVECTOR(0, 0, -40), new TV_3DVECTOR(0, 0, -2000))
        };

      Debris = new DebrisSpawnerData[] {
        new DebrisSpawnerData("XWRULD", new TV_3DVECTOR(-30, -10, 0), 0, 20, 10, 10, -25, 25, 0.5f),
        new DebrisSpawnerData("XWRDLU", new TV_3DVECTOR(30, -10, 0), -20, 0, -10, 10, -25, 25, 0.5f),
        };

      Loadouts = new string[] { "Z95__LASR" };
    }
  }
}

