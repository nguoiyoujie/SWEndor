﻿using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.ActorTypes.Components;
using SWEndor.Sound;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class MC90ATI : Groups.Warship
  {
    internal MC90ATI(Factory owner) : base(owner, "Mon Calamari Capital Ship")
    {
      Explodes = new ExplodeInfo[] {
        new ExplodeInfo("ExpL00", 0.5f, 1, ExplodeTrigger.ON_DYING | ExplodeTrigger.CREATE_ON_MESHVERTICES),
        new ExplodeInfo("ExpL01", 1, 3.5f, ExplodeTrigger.ON_DEATH),
        new ExplodeInfo("ExpW01", 1, 1, ExplodeTrigger.ON_DEATH)
      };

      MaxStrength = 3200.0f;
      ImpactDamage = 60.0f;
      MoveLimitData.MaxSpeed = 30.0f;
      MoveLimitData.MinSpeed = 0.0f;
      MoveLimitData.MaxSpeedChangeRate = 5.0f;
      MoveLimitData.MaxTurnRate = 2.6f;
      MoveLimitData.ZNormFrac = 0.006f;
      MoveLimitData.ZTilt = 2.2f; // low number so fighters come out properly when mothership is rotating

      RenderData.CullDistance = 30000;
      Scale = 1.8f;

      ScoreData = new ScoreData(20, 20000);

      RegenData = new RegenData(false, 0.35f, 0, 0, 0);

      MeshData = new MeshData(Name, @"mc90\mc90.x");
      DyingMoveData.Sink(0.01f, 2.5f, 0.4f);

      Cameras = new LookData[] { new LookData(new TV_3DVECTOR(0, 45, 660), new TV_3DVECTOR(0, 45, 2000)) };
      DeathCamera = new DeathCameraData(1500, 250, 30);
      SoundSources = new SoundSourceData[] { new SoundSourceData(SoundGlobals.EngineShip, 1500f, new TV_3DVECTOR(0, 0, -750), true) };
      AddOns = new AddOnData[]
      {
        new AddOnData("MC90 Turbolaser Tower", new TV_3DVECTOR(100, 75, 500), new TV_3DVECTOR(-80, 90, 0), true)
        , new AddOnData("MC90 Turbolaser Tower", new TV_3DVECTOR(-100, 75, 500), new TV_3DVECTOR(-80, -90, 0), true)
        , new AddOnData("MC90 Turbolaser Tower", new TV_3DVECTOR(0, 135, 0), new TV_3DVECTOR(-90, 0, 0), true)

        //bottom
        , new AddOnData("MC90 Turbolaser Tower", new TV_3DVECTOR(0, -40, 700), new TV_3DVECTOR(90, 0, 0), true)
        , new AddOnData("MC90 Turbolaser Tower", new TV_3DVECTOR(0, 40, 700), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnData("MC90 Turbolaser Tower", new TV_3DVECTOR(170, 40, 100), new TV_3DVECTOR(-75, 90, 0), true)
        , new AddOnData("MC90 Turbolaser Tower", new TV_3DVECTOR(-170, 40, 100), new TV_3DVECTOR(-75, -90, 0), true)
        , new AddOnData("MC90 Turbolaser Tower", new TV_3DVECTOR(210, 40, -300), new TV_3DVECTOR(-70, 90, 0), true)
        , new AddOnData("MC90 Turbolaser Tower", new TV_3DVECTOR(-210, 40, -300), new TV_3DVECTOR(-70, -90, 0), true)
        , new AddOnData("MC90 Turbolaser Tower", new TV_3DVECTOR(170, 40, -600), new TV_3DVECTOR(-75, 90, 0), true)
        , new AddOnData("MC90 Turbolaser Tower", new TV_3DVECTOR(-170, 40, -600), new TV_3DVECTOR(-75, -90, 0), true)

        // Hangar Bay
        , new AddOnData("Hangar Bay", new TV_3DVECTOR(95, -30, -200), new TV_3DVECTOR(0, 0, 0), true)
        , new AddOnData("Hangar Bay", new TV_3DVECTOR(-145, -25, -27), new TV_3DVECTOR(0, 180, 0), true)
        , new AddOnData("Hangar Bay", new TV_3DVECTOR(-145, -25, -230), new TV_3DVECTOR(0, 180, 0), true)
      };
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);
      ainfo.SpawnerInfo = new MC90Spawner(ainfo);
    }
  }
}

