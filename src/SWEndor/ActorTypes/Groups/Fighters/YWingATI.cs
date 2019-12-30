using MTV3D65;
using SWEndor.ActorTypes.Components;
using SWEndor.Sound;

namespace SWEndor.ActorTypes.Instances
{
  internal class YWingATI : Groups.RebelWing
  {
    internal YWingATI(Factory owner) : base(owner, "YWING", "Y-Wing")
    {
      SystemData.MaxShield = 16;
      SystemData.MaxHull = 8;
      CombatData.ImpactDamage = 16;
      MoveLimitData.MaxSpeed = 300;
      MoveLimitData.MinSpeed = 150;
      MoveLimitData.MaxSpeedChangeRate = 150;
      MoveLimitData.MaxTurnRate = 32;

      ScoreData = new ScoreData(400, 2000);

      RegenData = new RegenData(false, 0.16f, 0, 0, 0);

      MeshData = new MeshData(Engine, Name, @"ywing\ywing.x");
      SoundData.SoundSources = new SoundSourceData[] { new SoundSourceData(SoundGlobals.EngineYWing, 200f, new TV_3DVECTOR(0, 0, -30), true, isEngineSound: true) };

      CameraData.Cameras = new LookData[] {
        new LookData(new TV_3DVECTOR(0, 4, 30), new TV_3DVECTOR(0, 0, 2000)),
        new LookData(new TV_3DVECTOR(0, 25, -100), new TV_3DVECTOR(0, 0, 2000)),
        new LookData(new TV_3DVECTOR(0, 0, -40), new TV_3DVECTOR(0, 0, -2000))
      };

      DebrisData.Debris = new DebrisSpawnerData[] {
        new DebrisSpawnerData("YWWING", new TV_3DVECTOR(-30, 0, 0), -10, 10, 0, 30, -25, 25, 0.5f),
        new DebrisSpawnerData("YWWING", new TV_3DVECTOR(30, 0, 0), -10, 10, -30, 0, -25, 25, 0.5f),
        };

      WeapSystemData.Loadouts = new WeapData[]
      {
        new WeapData("TORP", "SEC_1_AI", "NO_AUTOAIM", "Y_WG_TORP", "Y_WG_TORP", "WING_TORP", "WING_TORP"),
        new WeapData("ION", "SEC_1_AI", "NO_AUTOAIM", "Y_WG_ION", "Y_WG_ION", "WING_LSR_ION", "WING_ION"),
        new WeapData("LASR", "PRI_1_AI", "NO_AUTOAIM", "DEFAULT", "Y_WG_LASR", "WING_LSR_R", "WING_LASER"),
      };
    }
  }
}

