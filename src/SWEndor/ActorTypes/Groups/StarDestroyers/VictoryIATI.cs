using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes.Components;
using SWEndor.Sound;

namespace SWEndor.ActorTypes.Instances
{
  internal class VictoryIATI : Groups.StarDestroyer
  {
    internal VictoryIATI(Factory owner) : base(owner, "VICT", "Victory-I Star Destroyer")
    {
      SystemData.MaxShield = 300;
      SystemData.MaxHull = 450;
      CombatData.ImpactDamage = 60.0f;
      MoveLimitData.MaxSpeed = 70.0f;
      MoveLimitData.MinSpeed = 0.0f;
      MoveLimitData.MaxSpeedChangeRate = 5.0f;
      MoveLimitData.MaxTurnRate = 1.2f;

      RenderData.CullDistance = 60000;
      ScoreData = new ScoreData(60, 10000);
      RegenData = new RegenData(false, 1.2f, 0, 0, 0);

      MeshData = new MeshData(Engine, Name, @"stardestroyer\victory_star_destroyer.x", 1.1f);
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
        , new AddOnData("IMPLMPOD", new TV_3DVECTOR(460, -40, -400), new TV_3DVECTOR(0, 72, 0), true)
        , new AddOnData("IMPLMPOD", new TV_3DVECTOR(425, -40, -280), new TV_3DVECTOR(0, 72, 0), true)
        , new AddOnData("IMPLMPOD", new TV_3DVECTOR(390, -40, -160), new TV_3DVECTOR(0, 72, 0), true)
        , new AddOnData("IMPLMPOD", new TV_3DVECTOR(355, -40, -40), new TV_3DVECTOR(0, 72, 0), true)
        , new AddOnData("IMPLMPOD", new TV_3DVECTOR(320, -40, 80), new TV_3DVECTOR(0, 72, 0), true)

        , new AddOnData("IMPLMPOD", new TV_3DVECTOR(-460, -40, -400), new TV_3DVECTOR(0, -72, 0), true)
        , new AddOnData("IMPLMPOD", new TV_3DVECTOR(-425, -40, -280), new TV_3DVECTOR(0, -72, 0), true)
        , new AddOnData("IMPLMPOD", new TV_3DVECTOR(-390, -40, -160), new TV_3DVECTOR(0, -72, 0), true)
        , new AddOnData("IMPLMPOD", new TV_3DVECTOR(-355, -40, -40), new TV_3DVECTOR(0, -72, 0), true)
        , new AddOnData("IMPLMPOD", new TV_3DVECTOR(-320, -40, 80), new TV_3DVECTOR(0, -72, 0), true)

        , new AddOnData("IMPLLSR", new TV_3DVECTOR(240, -40, 320), new TV_3DVECTOR(0, 72, 0), true)
        , new AddOnData("IMPLLSR", new TV_3DVECTOR(210, -40, 410), new TV_3DVECTOR(0, 72, 0), true)
        , new AddOnData("IMPLLSR", new TV_3DVECTOR(180, -40, 500), new TV_3DVECTOR(0, 72, 0), true)

        , new AddOnData("IMPLLSR", new TV_3DVECTOR(-240, -40, 320), new TV_3DVECTOR(0, -72, 0), true)
        , new AddOnData("IMPLLSR", new TV_3DVECTOR(-210, -40, 410), new TV_3DVECTOR(0, -72, 0), true)
        , new AddOnData("IMPLLSR", new TV_3DVECTOR(-180, -40, 500), new TV_3DVECTOR(0, -72, 0), true)
        
        // Top
        , new AddOnData("IMPLLSR", new TV_3DVECTOR(100, 20, 320), new TV_3DVECTOR(-75, 90, 0), true)

        , new AddOnData("IMPLLSR", new TV_3DVECTOR(-100, 20, 320), new TV_3DVECTOR(-75, -90, 0), true)

        // Top Middle
        , new AddOnData("IMPLHLSR", new TV_3DVECTOR(72, 35, 75), new TV_3DVECTOR(-75, 90, 0), true)

        , new AddOnData("IMPLHLSR", new TV_3DVECTOR(-72, 35, 75), new TV_3DVECTOR(-75, -90, 0), true)

        // Top Rear
        , new AddOnData("IMPLHLSR", new TV_3DVECTOR(290, 16, -230), new TV_3DVECTOR(-75, 90, 0), true)
        , new AddOnData("IMPLHLSR", new TV_3DVECTOR(290, 18, -283), new TV_3DVECTOR(-75, 90, 0), true)
        , new AddOnData("IMPLMPOD2", new TV_3DVECTOR(365, 10, -480), new TV_3DVECTOR(-75, 90, 0), true)

        , new AddOnData("IMPLHLSR", new TV_3DVECTOR(-290, 16, -230), new TV_3DVECTOR(-75, -90, 0), true)
        , new AddOnData("IMPLHLSR", new TV_3DVECTOR(-290, 18, -283), new TV_3DVECTOR(-75, -90, 0), true)
        , new AddOnData("IMPLMPOD2", new TV_3DVECTOR(-365, 10, -480), new TV_3DVECTOR(-75, -90, 0), true)

        // Bottom
        , new AddOnData("IMPLLSR", new TV_3DVECTOR(-120, -105, 180), new TV_3DVECTOR(65, -90, 0), true)
        , new AddOnData("IMPLLSR", new TV_3DVECTOR(-360, -100, -400), new TV_3DVECTOR(65, -90, 0), true)

        , new AddOnData("IMPLLSR", new TV_3DVECTOR(120, -105, 180), new TV_3DVECTOR(65, 90, 0), true)
        , new AddOnData("IMPLLSR", new TV_3DVECTOR(360, -100, -425), new TV_3DVECTOR(65, 90, 0), true)

        // Hangar Bay
        , new AddOnData("HANGAR", new TV_3DVECTOR(0, -80, 105), new TV_3DVECTOR(0, 0, 0), true)

        //Shield Generators
        , new AddOnData("SHD", new TV_3DVECTOR(-120, 310, -415), new TV_3DVECTOR(0, 0, 0), true)
        , new AddOnData("SHD", new TV_3DVECTOR(120, 310, -415), new TV_3DVECTOR(0, 0, 0), true)
      };
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);
      ainfo.SpawnerInfo = SpawnerInfoDecorator.SD_Default;
    }
  }
}

