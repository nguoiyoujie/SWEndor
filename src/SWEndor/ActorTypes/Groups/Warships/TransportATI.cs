using MTV3D65;
using SWEndor.ActorTypes.Components;
using SWEndor.Sound;

namespace SWEndor.ActorTypes.Instances
{
  internal class TransportATI : Groups.Warship
  {
    internal TransportATI(Factory owner) : base(owner, "TRAN", "Transport")
    {
      SystemData.MaxShield = 100;
      SystemData.MaxHull = 150;
      CombatData.ImpactDamage = 60.0f;
      MoveLimitData.MaxSpeed = 40.0f;
      MoveLimitData.MinSpeed = 0.0f;
      MoveLimitData.MaxSpeedChangeRate = 5.0f;
      MoveLimitData.MaxTurnRate = 3f;

      RenderData.CullDistance = 20000;
      ScoreData = new ScoreData(8, 3000);

      MeshData = new MeshData(Engine, Name, @"transport\transport.x", 0.5f);

      Cameras = new LookData[] { new LookData(new TV_3DVECTOR(0, 86, -150), new TV_3DVECTOR(0, 86, 2000)) };
      Debris = new DebrisSpawnerData[] {
        new DebrisSpawnerData("BOX1", new TV_3DVECTOR(60, -30, 1), -10, 10, -20, 20, -25, 25, 0.5f),
        new DebrisSpawnerData("BOX1", new TV_3DVECTOR(0, -30, 1), -10, 10, -20, 20, -25, 25, 0.5f),
        new DebrisSpawnerData("BOX1", new TV_3DVECTOR(-60, -30, 1), -10, 10, -20, 20, -25, 25, 0.5f),
        new DebrisSpawnerData("BOX2", new TV_3DVECTOR(60, -30, 1), -10, 10, -20, 20, -25, 25, 0.5f),
        new DebrisSpawnerData("BOX2", new TV_3DVECTOR(0, -30, 1), -10, 10, -20, 20, -25, 25, 0.5f),
        new DebrisSpawnerData("BOX2", new TV_3DVECTOR(-60, -30, 1), -10, 10, -20, 20, -25, 25, 0.5f),
        new DebrisSpawnerData("BOX3", new TV_3DVECTOR(60, -30, 1), -10, 10, -20, 20, -25, 25, 0.5f),
        new DebrisSpawnerData("BOX3", new TV_3DVECTOR(0, -30, 1), -10, 10, -20, 20, -25, 25, 0.5f),
        new DebrisSpawnerData("BOX3", new TV_3DVECTOR(-60, -30, 1), -10, 10, -20, 20, -25, 25, 0.5f),
        new DebrisSpawnerData("BOX4", new TV_3DVECTOR(60, -30, 1), -10, 10, -20, 20, -25, 25, 0.5f),
        new DebrisSpawnerData("BOX4", new TV_3DVECTOR(0, -30, 1), -10, 10, -20, 20, -25, 25, 0.5f),
        new DebrisSpawnerData("BOX4", new TV_3DVECTOR(-60, -30, 1), -10, 10, -20, 20, -25, 25, 0.5f),
      };

      SoundSources = new SoundSourceData[] { new SoundSourceData(SoundGlobals.EngineShip, 500.0f, new TV_3DVECTOR(0, 0, -150), true, isEngineSound: true) };
      AddOns = new AddOnData[]
      {
        new AddOnData("TRANLSR", new TV_3DVECTOR(0, 70, 200), new TV_3DVECTOR(-90, 0, 0), true)
      };
    }
  }
}

