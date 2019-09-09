using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class InterdictorATI : Groups.StarDestroyer
  {
    internal InterdictorATI(Factory owner) : base(owner, "Interdictor Star Destroyer")
    {
      MaxStrength = 550.0f;
      ImpactDamage = 60.0f;
      MoveLimitData.MaxSpeed = 60.0f;
      MoveLimitData.MinSpeed = 0.0f;
      MoveLimitData.MaxSpeedChangeRate = 5.0f;
      MoveLimitData.MaxTurnRate = 1.2f;

      RenderData.CullDistance = 40000;
      Scale = 1.1f;

      ScoreData = new ScoreData(60, 10000);

      MeshData = new MeshData(Name, @"stardestroyer\interdictor_star_destroyer.x");
      //SourceFarMeshPath = Path.Combine(Globals.ModelPath, @"stardestroyer\star_destroyer_far.x");

      Cameras = new LookData[] {
        new LookData(new TV_3DVECTOR(0, 300, -385), new TV_3DVECTOR(0, 300, 2000)),
        };
      DeathCamera = new DeathCameraData(1750, 400, 30);
      SoundSources = new SoundSourceData[] { new SoundSourceData("engine_big", 1500f, new TV_3DVECTOR(0, -100, -400), true) };
      AddOns = new AddOnData[]
      {
        // Front
        new AddOnData("Star Destroyer Turbolaser Tower", new TV_3DVECTOR(0, -40, 1040), new TV_3DVECTOR(0, 0, 0), true)

        // Sides
        , new AddOnData("Star Destroyer Turbolaser Tower", new TV_3DVECTOR(360, -40, -100), new TV_3DVECTOR(0, 72, 0), true)
        , new AddOnData("Star Destroyer Turbolaser Tower", new TV_3DVECTOR(240, -40, 320), new TV_3DVECTOR(0, 72, 0), true)
        , new AddOnData("Star Destroyer Heavy Turbolaser Tower", new TV_3DVECTOR(210, -40, 410), new TV_3DVECTOR(0, 72, 0), true)
        , new AddOnData("Star Destroyer Turbolaser Tower", new TV_3DVECTOR(180, -40, 500), new TV_3DVECTOR(0, 72, 0), true)

        , new AddOnData("Star Destroyer Turbolaser Tower", new TV_3DVECTOR(-360, -40, -100), new TV_3DVECTOR(0, -72, 0), true)
        , new AddOnData("Star Destroyer Turbolaser Tower", new TV_3DVECTOR(-240, -40, 320), new TV_3DVECTOR(0, -72, 0), true)
        , new AddOnData("Star Destroyer Heavy Turbolaser Tower", new TV_3DVECTOR(-210, -40, 410), new TV_3DVECTOR(0, -72, 0), true)
        , new AddOnData("Star Destroyer Turbolaser Tower", new TV_3DVECTOR(-180, -40, 500), new TV_3DVECTOR(0, -72, 0), true)
        
        // Top
        , new AddOnData("Star Destroyer Heavy Turbolaser Tower", new TV_3DVECTOR(100, 20, 320), new TV_3DVECTOR(-90, 0, -15), true)
        , new AddOnData("Star Destroyer Turbolaser Tower", new TV_3DVECTOR(72, 20, 420), new TV_3DVECTOR(-90, 0, -15), true)

        , new AddOnData("Star Destroyer Heavy Turbolaser Tower", new TV_3DVECTOR(-100, 20, 320), new TV_3DVECTOR(-90, 0, 15), true)
        , new AddOnData("Star Destroyer Turbolaser Tower", new TV_3DVECTOR(-72, 20, 420), new TV_3DVECTOR(-90, 0, 15), true)

        // Bottom
        , new AddOnData("Star Destroyer Turbolaser Tower", new TV_3DVECTOR(-120, -105, 180), new TV_3DVECTOR(90, 0, -25), true)
        , new AddOnData("Star Destroyer Turbolaser Tower", new TV_3DVECTOR(-360, -100, -400), new TV_3DVECTOR(90, 0, -25), true)

        , new AddOnData("Star Destroyer Turbolaser Tower", new TV_3DVECTOR(120, -105, 180), new TV_3DVECTOR(90, 0, 25), true)
        , new AddOnData("Star Destroyer Turbolaser Tower", new TV_3DVECTOR(360, -100, -425), new TV_3DVECTOR(90, 0, 25), true)

        // Hangar Bay
        , new AddOnData("Hangar Bay", new TV_3DVECTOR(0, -80, 105), new TV_3DVECTOR(0, 0, 0), true)

        //Shield Generators
        , new AddOnData("Star Destroyer Shield Generator", new TV_3DVECTOR(-120, 325, -415), new TV_3DVECTOR(0, 0, 0), true)
        , new AddOnData("Star Destroyer Shield Generator", new TV_3DVECTOR(120, 325, -415), new TV_3DVECTOR(0, 0, 0), true)
        //, new AddOnInfo("Star Destroyer Lower Shield Generator", new TV_3DVECTOR(0, -180, -250), new TV_3DVECTOR(0, 0, 0), true)
      };
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);
      ainfo.DyingMoveComponent = new DyingSink(0.005f, 5f, 0.8f);
      ainfo.SpawnerInfo = new SDSpawner(ainfo);
    }
  }
}

