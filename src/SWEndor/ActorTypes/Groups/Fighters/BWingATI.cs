using MTV3D65;
using SWEndor.ActorTypes.Components;
using SWEndor.Sound;

namespace SWEndor.ActorTypes.Instances
{
  internal class BWingATI : Groups.RebelWing
  {
    internal BWingATI(Factory owner) : base(owner, "BWING", "B-Wing")
    {
      SystemData.MaxShield = 30;
      SystemData.MaxHull = 10;
      CombatData.ImpactDamage = 16;
      MoveLimitData.MaxSpeed = 400;
      MoveLimitData.MinSpeed = 200;
      MoveLimitData.MaxSpeedChangeRate = 200;
      MoveLimitData.MaxTurnRate = 40;

      ScoreData = new ScoreData(400, 2500);

      RegenData = new RegenData(false, 0.16f, 0, 0, 0);

      MeshData = new MeshData(Engine, Name, @"bwing\bwing.x");

      Cameras = new LookData[] {
        new LookData(new TV_3DVECTOR(0, 0, 14), new TV_3DVECTOR(0, 0, 2000)),
        new LookData(new TV_3DVECTOR(0, 25, -100), new TV_3DVECTOR(0, 0, 2000)),
        new LookData(new TV_3DVECTOR(0, 0, -40), new TV_3DVECTOR(0, 0, -2000))
     };

      Debris = new DebrisSpawnerData[] {
        new DebrisSpawnerData("BWWING", new TV_3DVECTOR(-30, -30, 0), -10, 10, 0, 30, -25, 25, 0.5f),
        new DebrisSpawnerData("BWWING", new TV_3DVECTOR(30, -30, 0), -10, 10, -30, 0, -25, 25, 0.5f),
        new DebrisSpawnerData("BWWTOP", new TV_3DVECTOR(0, 0, 0), -10, 10, -10, 10, -25, 25, 0.5f),
        new DebrisSpawnerData("BWWBOT", new TV_3DVECTOR(0, -70, 0), -10, 10, -10, 10, -25, 25, 0.5f)
        };

      SoundSources = new SoundSourceData[] { new SoundSourceData(SoundGlobals.EngineXWing, 200f, new TV_3DVECTOR(0, 0, -30), true, isEngineSound: true) };

      Loadouts = new WeapData[]
      {
        new WeapData("TORP", "SEC_1_AI", "NO_AUTOAIM", "B_WG_TORP", "B_WG_TORP", "WING_TORP", "WING_TORP"),
        new WeapData("ION", "SEC_1_AI", "NO_AUTOAIM", "B_WG_ION", "B_WG_ION", "WING_LSR_ION", "WING_ION"),
        new WeapData("LASR", "PRI_124_AI", "NO_AUTOAIM", "DEFAULT", "B_WG_LASR", "WING_LSR_R", "WING_LASER"),
      };
    }
  }
}

