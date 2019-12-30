﻿using MTV3D65;
using SWEndor.ActorTypes.Components;
using SWEndor.Models;
using SWEndor.Sound;

namespace SWEndor.ActorTypes.Instances
{
  internal class MC80BATI : Groups.Warship
  {
    internal MC80BATI(Factory owner) : base(owner, "MC80B", "Mon Calamari 80B Capital Ship")
    {
      ExplodeSystemData.Explodes = new ExplodeData[] {
        new ExplodeData("EXPL00", 0.5f, 1, ExplodeTrigger.ON_DYING | ExplodeTrigger.CREATE_ON_MESHVERTICES),
        new ExplodeData("EXPL01", 1, 3.5f, ExplodeTrigger.ON_DEATH),
        new ExplodeData("EXPW01", 1, 1, ExplodeTrigger.ON_DEATH)
      };

      SystemData.MaxShield = 2000;
      SystemData.MaxHull = 800;
      CombatData.ImpactDamage = 60.0f;
      MoveLimitData.MaxSpeed = 30.0f;
      MoveLimitData.MinSpeed = 0.0f;
      MoveLimitData.MaxSpeedChangeRate = 5.0f;
      MoveLimitData.MaxTurnRate = 3f;
      MoveLimitData.ZNormFrac = 0.006f;
      MoveLimitData.ZTilt = 3.5f;

      ScoreData = new ScoreData(20, 20000);
      RenderData.CullDistance = 60000;

      RegenData = new RegenData(false, 3.2f, 0, 0, 0);

      MeshData = new MeshData(Engine, Name, @"mc90\mc80b.x", 1.8f);
      DyingMoveData.Sink(0.01f, 2.5f, 0.4f);

      CameraData.Cameras = new LookData[] { new LookData(new TV_3DVECTOR(0, 45, 660), new TV_3DVECTOR(0, 45, 2000)) };
      CameraData.DeathCamera = new DeathCameraData(1500, 250, 30);
      SoundData.SoundSources = new SoundSourceData[] { new SoundSourceData(SoundGlobals.EngineShip, 1500f, new TV_3DVECTOR(0, 0, -750), true, isEngineSound: true) };
      AddOnData.AddOns = new AddOnData[]
      {
        new AddOnData("MC90LSR", new TV_3DVECTOR(0, 45, 1200), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnData("MC90LSR", new TV_3DVECTOR(-120, 42, 950), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnData("MC90LSR", new TV_3DVECTOR(120, 42, 950), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnData("MC90LSR", new TV_3DVECTOR(-180, 48, 520), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnData("MC90LSR", new TV_3DVECTOR(180, 48, 520), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnData("MC90LSR", new TV_3DVECTOR(-180, -65, 410), new TV_3DVECTOR(90, 0, 0), true)
        , new AddOnData("MC90LSR", new TV_3DVECTOR(180, -65, 410), new TV_3DVECTOR(90, 0, 0), true)

        , new AddOnData("MC90LSR", new TV_3DVECTOR(-220, 52, 300), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnData("MC90LSR", new TV_3DVECTOR(220, 52, 300), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnData("MC90LSR", new TV_3DVECTOR(-210, -75, 150), new TV_3DVECTOR(90, 0, 0), true)
        , new AddOnData("MC90LSR", new TV_3DVECTOR(210, -75, 150), new TV_3DVECTOR(90, 0, 0), true)

        , new AddOnData("MC90LSR", new TV_3DVECTOR(0, 120, -225), new TV_3DVECTOR(-90, 0, 0), true)
      };
    }
  }
}

