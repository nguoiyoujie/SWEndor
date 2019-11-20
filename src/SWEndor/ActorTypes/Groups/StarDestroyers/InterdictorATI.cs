using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes.Components;
using SWEndor.Core;
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

      RenderData.CullDistance = 40000;
      ScoreData = new ScoreData(60, 10000);
      RegenData = new RegenData(false, 0.5f, 0, 0, 0);

      MeshData = new MeshData(Engine, Name, @"stardestroyer\interdictor_star_destroyer.x", 1.4f);
      DyingMoveData.Sink(0.005f, 5f, 0.8f);

      Cameras = new LookData[] {
        new LookData(new TV_3DVECTOR(0, 300, -385), new TV_3DVECTOR(0, 300, 2000)),
        };
      DeathCamera = new DeathCameraData(1750, 400, 30);
      SoundSources = new SoundSourceData[] { new SoundSourceData(SoundGlobals.EngineShip, 1500f, new TV_3DVECTOR(0, -100, -400), true, isEngineSound: true) };
      AddOns = new AddOnData[]
      {
        // Front
        new AddOnData("IMPLLSR", new TV_3DVECTOR(0, -40, 1040), new TV_3DVECTOR(0, 0, 0), true)

        // Sides
        , new AddOnData("IMPLLSR", new TV_3DVECTOR(360, -40, -100), new TV_3DVECTOR(0, 72, 0), true)
        , new AddOnData("IMPLLSR", new TV_3DVECTOR(240, -40, 320), new TV_3DVECTOR(0, 72, 0), true)
        , new AddOnData("IMPLHLSR", new TV_3DVECTOR(210, -40, 410), new TV_3DVECTOR(0, 72, 0), true)
        , new AddOnData("IMPLLSR", new TV_3DVECTOR(180, -40, 500), new TV_3DVECTOR(0, 72, 0), true)

        , new AddOnData("IMPLLSR", new TV_3DVECTOR(-360, -40, -100), new TV_3DVECTOR(0, -72, 0), true)
        , new AddOnData("IMPLLSR", new TV_3DVECTOR(-240, -40, 320), new TV_3DVECTOR(0, -72, 0), true)
        , new AddOnData("IMPLHLSR", new TV_3DVECTOR(-210, -40, 410), new TV_3DVECTOR(0, -72, 0), true)
        , new AddOnData("IMPLLSR", new TV_3DVECTOR(-180, -40, 500), new TV_3DVECTOR(0, -72, 0), true)
        
        // Top
        , new AddOnData("IMPLHLSR", new TV_3DVECTOR(100, 20, 320), new TV_3DVECTOR(-75, 90, 0), true)
        , new AddOnData("IMPLLSR", new TV_3DVECTOR(72, 20, 420), new TV_3DVECTOR(-75, 90, 0), true)

        , new AddOnData("IMPLHLSR", new TV_3DVECTOR(-100, 20, 320), new TV_3DVECTOR(-75, -90, 0), true)
        , new AddOnData("IMPLLSR", new TV_3DVECTOR(-72, 20, 420), new TV_3DVECTOR(-75, -90, 0), true)

        // Bottom
        , new AddOnData("IMPLLSR", new TV_3DVECTOR(-120, -105, 180), new TV_3DVECTOR(65, -90, 0), true)
        , new AddOnData("IMPLLSR", new TV_3DVECTOR(-360, -100, -400), new TV_3DVECTOR(65, -90, 0), true)

        , new AddOnData("IMPLLSR", new TV_3DVECTOR(120, -105, 180), new TV_3DVECTOR(65, 90, 0), true)
        , new AddOnData("IMPLLSR", new TV_3DVECTOR(360, -100, -425), new TV_3DVECTOR(65, 90, 0), true)

        // Hangar Bay
        , new AddOnData("HANGAR", new TV_3DVECTOR(0, -80, 105), new TV_3DVECTOR(0, 0, 0), true)

        //Shield Generators
        , new AddOnData("SHD", new TV_3DVECTOR(-120, 325, -415), new TV_3DVECTOR(0, 0, 0), true)
        , new AddOnData("SHD", new TV_3DVECTOR(120, 325, -415), new TV_3DVECTOR(0, 0, 0), true)
        //, new AddOnInfo("LSHD", new TV_3DVECTOR(0, -180, -250), new TV_3DVECTOR(0, 0, 0), true)
      };
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);
      ainfo.SpawnerInfo = SpawnerInfoDecorator.SD_Default;
    }
  }
}

