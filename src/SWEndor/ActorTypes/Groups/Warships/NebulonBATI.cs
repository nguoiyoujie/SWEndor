using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes.Components;
using SWEndor.Sound;

namespace SWEndor.ActorTypes.Instances
{
  internal class NebulonBATI : Groups.Warship
  {
    internal NebulonBATI(Factory owner) : base(owner, "NEBL", "Nebulon-B Frigate")
    {
      SystemData.MaxShield = 400;
      SystemData.MaxHull = 550;
      CombatData.ImpactDamage = 60.0f;
      MoveLimitData.MaxSpeed = 32.0f;
      MoveLimitData.MinSpeed = 0.0f;
      MoveLimitData.MaxSpeedChangeRate = 5.0f;
      MoveLimitData.MaxTurnRate = 2f;

      ScoreData = new ScoreData(15, 10000);
      RenderData.CullDistance = 45000;
      RegenData = new RegenData(false, 1.2f, 0, 0, 0);

      MeshData = new MeshData(Engine, Name, @"nebulonb\nebulonb.x", 0.75f);
      DyingMoveData.Sink(0.02f, 5f, 0.8f);

      CameraData.Cameras = new LookData[] { new LookData(new TV_3DVECTOR(66, 78, -480), new TV_3DVECTOR(0, 75, 2000)) };
      SoundData.SoundSources = new SoundSourceData[] { new SoundSourceData(SoundGlobals.EngineShip, 1500.0f, new TV_3DVECTOR(0, 100, -300), true, isEngineSound: true) };
      AddOnData.AddOns = new AddOnData[]
      {
        new AddOnData("NEBLLSR", new TV_3DVECTOR(0, 40, 220), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnData("NEBLLSR", new TV_3DVECTOR(56, 90, -520), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnData("NEBLLSR", new TV_3DVECTOR(0, -180, -550), new TV_3DVECTOR(90, 0, 0), true)
        , new AddOnData("NEBLMPOD", new TV_3DVECTOR(-80, -45, -485), new TV_3DVECTOR(0, 0, 0), true)
        , new AddOnData("NEBLMPOD", new TV_3DVECTOR(80, -45, -485), new TV_3DVECTOR(0, 0, 0), true)

        , new AddOnData("HANGAR", new TV_3DVECTOR(10, -60, 192), new TV_3DVECTOR(0, 180, 0), true)
      };

      SpawnerData.SpawnTypes = new string[] { "Z95", "YWING" };
      SpawnerData.SpawnMoveDelay = 2;
      SpawnerData.SpawnInterval = 10;
      SpawnerData.SpawnsRemaining = 99;
      SpawnerData.SpawnLocations = new TV_3DVECTOR[] { new TV_3DVECTOR(-20, 0, 0) };
      SpawnerData.SpawnSpeed = 25;
      SpawnerData.SpawnRotation = new TV_3DVECTOR(0, 90, 0);
    }
  }
}

