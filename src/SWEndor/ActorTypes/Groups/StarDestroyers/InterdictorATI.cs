using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes.Components;
using SWEndor.Sound;

namespace SWEndor.ActorTypes.Instances
{
  internal class InterdictorATI : Groups.StarDestroyer
  {
    internal InterdictorATI(Factory owner) : base(owner, "INTD", "Interdictor Star Destroyer")
    {
      SystemData.MaxShield = 300;
      SystemData.MaxHull = 450;
      CombatData.ImpactDamage = 60.0f;
      MoveLimitData.MaxSpeed = 60.0f;
      MoveLimitData.MinSpeed = 0.0f;
      MoveLimitData.MaxSpeedChangeRate = 5.0f;
      MoveLimitData.MaxTurnRate = 1.2f;

      RenderData.CullDistance = 50000;
      ScoreData = new ScoreData(60, 10000);
      RegenData = new RegenData(false, 1.2f, 0, 0, 0);

      MeshData = new MeshData(Engine, Name, @"stardestroyer\interdictor_star_destroyer.x", 1.4f);
      DyingMoveData.Sink(0.005f, 5f, 0.8f);

      CameraData.Cameras = new LookData[] {
        new LookData(new TV_3DVECTOR(0, 300, -385), new TV_3DVECTOR(0, 300, 2000)),
        };
      CameraData.DeathCamera = new DeathCameraData(1750, 400, 30);
      SoundData.SoundSources = new SoundSourceData[] { new SoundSourceData(SoundGlobals.EngineShip, 1500f, new TV_3DVECTOR(0, -100, -400), true, isEngineSound: true) };
      AddOnData.AddOns = new AddOnData[]
      {
        // Front
        new AddOnData("ISD_LSR", new TV_3DVECTOR(0, -40, 1040), new TV_3DVECTOR(0, 0, 0), true)

        // Sides
        , new AddOnData("ISD_LSR", new TV_3DVECTOR(360, -40, -100), new TV_3DVECTOR(0, 72, 0), true)
        , new AddOnData("ISD_LSR", new TV_3DVECTOR(240, -40, 320), new TV_3DVECTOR(0, 72, 0), true)
        , new AddOnData("ISD_HLSR", new TV_3DVECTOR(210, -40, 410), new TV_3DVECTOR(0, 72, 0), true)
        , new AddOnData("ISD_LSR", new TV_3DVECTOR(180, -40, 500), new TV_3DVECTOR(0, 72, 0), true)

        , new AddOnData("ISD_LSR", new TV_3DVECTOR(-360, -40, -100), new TV_3DVECTOR(0, -72, 0), true)
        , new AddOnData("ISD_LSR", new TV_3DVECTOR(-240, -40, 320), new TV_3DVECTOR(0, -72, 0), true)
        , new AddOnData("ISD_HLSR", new TV_3DVECTOR(-210, -40, 410), new TV_3DVECTOR(0, -72, 0), true)
        , new AddOnData("ISD_LSR", new TV_3DVECTOR(-180, -40, 500), new TV_3DVECTOR(0, -72, 0), true)
        
        // Top
        , new AddOnData("ISD_HLSR", new TV_3DVECTOR(100, 20, 320), new TV_3DVECTOR(-75, 90, 0), true)
        , new AddOnData("ISD_LSR", new TV_3DVECTOR(72, 20, 420), new TV_3DVECTOR(-75, 90, 0), true)

        , new AddOnData("ISD_HLSR", new TV_3DVECTOR(-100, 20, 320), new TV_3DVECTOR(-75, -90, 0), true)
        , new AddOnData("ISD_LSR", new TV_3DVECTOR(-72, 20, 420), new TV_3DVECTOR(-75, -90, 0), true)

        // Bottom
        , new AddOnData("ISD_LSR", new TV_3DVECTOR(-120, -105, 180), new TV_3DVECTOR(65, -90, 0), true)
        , new AddOnData("ISD_LSR", new TV_3DVECTOR(-360, -100, -400), new TV_3DVECTOR(65, -90, 0), true)

        , new AddOnData("ISD_LSR", new TV_3DVECTOR(120, -105, 180), new TV_3DVECTOR(65, 90, 0), true)
        , new AddOnData("ISD_LSR", new TV_3DVECTOR(360, -100, -425), new TV_3DVECTOR(65, 90, 0), true)

        // Hangar Bay
        , new AddOnData("HANGAR", new TV_3DVECTOR(0, -80, 105), new TV_3DVECTOR(0, 0, 0), true)

        //Shield Generators
        , new AddOnData("SHD", new TV_3DVECTOR(-120, 325, -415), new TV_3DVECTOR(0, 0, 0), true)
        , new AddOnData("SHD", new TV_3DVECTOR(120, 325, -415), new TV_3DVECTOR(0, 0, 0), true)
        //, new AddOnInfo("LSHD", new TV_3DVECTOR(0, -180, -250), new TV_3DVECTOR(0, 0, 0), true)
      };

      SpawnerData.SpawnTypes = new string[] { "TIE" };
      SpawnerData.SpawnMoveDelay = 4;
      SpawnerData.SpawnInterval = 5;
      SpawnerData.SpawnsRemaining = 60;
      SpawnerData.SpawnLocations = new TV_3DVECTOR[]{ new TV_3DVECTOR(50, 0, -50)
                                               , new TV_3DVECTOR(50, 0, 50)
                                               , new TV_3DVECTOR(-50, 0, -50)
                                               , new TV_3DVECTOR(-50, 0, 50)
                                               };

      SpawnerData.SpawnSpeed = -1;
      SpawnerData.SpawnManualPositioningMult = new TV_3DVECTOR(0, -30, 0);
    }
  }
}

