﻿using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Actors.Models;
using SWEndor.ActorTypes.Components;
using SWEndor.Sound;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class DevastatorATI : Groups.StarDestroyer
  {
    internal DevastatorATI(Factory owner) : base(owner, "Devastator Imperial-I Star Destroyer")
    {
      // Combat
      Explodes = new ExplodeInfo[] {
        new ExplodeInfo("ExpL00", 0.25f, 1, ExplodeTrigger.ON_DYING | ExplodeTrigger.CREATE_ON_MESHVERTICES),
        new ExplodeInfo("ExpL01", 1, 2.5f, ExplodeTrigger.ON_DEATH),
        new ExplodeInfo("ExpW01", 1, 1, ExplodeTrigger.ON_DEATH)
      };

      MaxStrength = 1075.0f;
      ImpactDamage = 60.0f;
      MoveLimitData.MaxSpeed = 70.0f;
      MoveLimitData.MinSpeed = 0.0f;
      MoveLimitData.MaxSpeedChangeRate = 5.0f;
      MoveLimitData.MaxTurnRate = 1.2f;

      RenderData.CullDistance = 40000;
      Scale = 1.7f;

      ScoreData = new ScoreData(70, 20000);

      MeshData = new MeshData(Name, @"stardestroyer\star_destroyer.x");
      DyingMoveData.Sink(0.005f, 5f, 0.8f);

      Cameras = new LookData[] {
        new LookData(new TV_3DVECTOR(0, 300, -385), new TV_3DVECTOR(0, 300, 2000)),
        };
      DeathCamera = new DeathCameraData(1750, 400, 30);
      SoundSources = new SoundSourceData[] { new SoundSourceData(SoundGlobals.EngineShip, 1500f, new TV_3DVECTOR(0, -100, -400), true) };
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
        , new AddOnData("Star Destroyer Heavy Turbolaser Tower", new TV_3DVECTOR(100, 20, 320), new TV_3DVECTOR(-75, 90, 0), true)
        , new AddOnData("Star Destroyer Turbolaser Tower", new TV_3DVECTOR(72, 20, 420), new TV_3DVECTOR(-75, 90, 0), true)

        , new AddOnData("Star Destroyer Heavy Turbolaser Tower", new TV_3DVECTOR(-100, 20, 320), new TV_3DVECTOR(-75, -90, 0), true)
        , new AddOnData("Star Destroyer Turbolaser Tower", new TV_3DVECTOR(-72, 20, 420), new TV_3DVECTOR(-75, -90, 0), true)

        // Top Middle
        , new AddOnData("Star Destroyer Heavy Turbolaser Tower", new TV_3DVECTOR(72, 35, 75), new TV_3DVECTOR(-75, 90, 0), true)
        , new AddOnData("Star Destroyer Turbolaser Tower", new TV_3DVECTOR(70, 35, 170), new TV_3DVECTOR(-75, 90, 0), true)

        , new AddOnData("Star Destroyer Heavy Turbolaser Tower", new TV_3DVECTOR(-72, 35, 75), new TV_3DVECTOR(-75, -90, 0), true)
        , new AddOnData("Star Destroyer Turbolaser Tower", new TV_3DVECTOR(-70, 35, 170), new TV_3DVECTOR(-75, -90, 0), true)

        // Top Rear
        , new AddOnData("Star Destroyer Heavy Turbolaser Tower", new TV_3DVECTOR(290, 16, -230), new TV_3DVECTOR(-75, 90, 0), true)
        , new AddOnData("Star Destroyer Heavy Turbolaser Tower", new TV_3DVECTOR(290, 18, -283), new TV_3DVECTOR(-75, 90, 0), true)
        , new AddOnData("Star Destroyer Missile Pod", new TV_3DVECTOR(290, 20, -336), new TV_3DVECTOR(-75, 90, 0), true)
        , new AddOnData("Star Destroyer Missile Pod", new TV_3DVECTOR(290, 22, -389), new TV_3DVECTOR(-75, 90, 0), true)

        , new AddOnData("Star Destroyer Heavy Turbolaser Tower", new TV_3DVECTOR(-290, 16, -230), new TV_3DVECTOR(-75, -90, 0), true)
        , new AddOnData("Star Destroyer Heavy Turbolaser Tower", new TV_3DVECTOR(-290, 18, -283), new TV_3DVECTOR(-75, -90, 0), true)
        , new AddOnData("Star Destroyer Missile Pod", new TV_3DVECTOR(-290, 20, -336), new TV_3DVECTOR(-75, -90, 0), true)
        , new AddOnData("Star Destroyer Missile Pod", new TV_3DVECTOR(-290, 22, -389), new TV_3DVECTOR(-75, -90, 0), true)

        // Bottom
        , new AddOnData("Star Destroyer Turbolaser Tower", new TV_3DVECTOR(-120, -105, 180), new TV_3DVECTOR(65, -90, 0), true)
        , new AddOnData("Star Destroyer Turbolaser Tower", new TV_3DVECTOR(-360, -100, -400), new TV_3DVECTOR(65, -90, 0), true)

        , new AddOnData("Star Destroyer Turbolaser Tower", new TV_3DVECTOR(120, -105, 180), new TV_3DVECTOR(65, 90, 0), true)
        , new AddOnData("Star Destroyer Turbolaser Tower", new TV_3DVECTOR(360, -100, -425), new TV_3DVECTOR(65, 90, 0), true)

        // Hangar Bay
        , new AddOnData("Hangar Bay", new TV_3DVECTOR(0, -80, 105), new TV_3DVECTOR(0, 0, 0), true)

        //Shield Generators
        , new AddOnData("Imperial Star Destroyer Shield Generator", new TV_3DVECTOR(-120, 360, -415), new TV_3DVECTOR(0, 0, 0), true)
        , new AddOnData("Imperial Star Destroyer Shield Generator", new TV_3DVECTOR(120, 360, -415), new TV_3DVECTOR(0, 0, 0), true)
       //, new AddOnInfo("Star Destroyer Lower Shield Generator", new TV_3DVECTOR(0, -180, -250), new TV_3DVECTOR(0, 0, 0), true)
     };
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);
      ainfo.SpawnerInfo = new SDSpawnerII(ainfo);
    }
  }
}

