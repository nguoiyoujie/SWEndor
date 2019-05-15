﻿using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Actors.Data;
using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class ExecutorATI : Groups.StarDestroyer
  {
    internal ExecutorATI(Factory owner) : base(owner, "Executor Super Star Destroyer")
    {
      ExplodeData = new ExplodeData(0.5f, 1, "ExplosionSm", DeathExplosionTrigger.ALWAYS, 5, "ExplosionLg");

      MaxStrength = 3500.0f;
      ImpactDamage = 120.0f;
      MaxSpeed = 30;
      MinSpeed = 0.0f;
      MaxSpeedChangeRate = 2.0f;
      MaxTurnRate = 2f;

      CullDistance = 60000;
      Scale = 2.5f;

      Score_perStrength = 75;
      Score_DestroyBonus = 100000;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"executor\executor.x");

      Cameras = new ActorCameraInfo[] {
        new ActorCameraInfo(new TV_3DVECTOR(0, 325, -840), new TV_3DVECTOR(0, 325, 2000)),
        };
      DeathCamera = new DeathCameraInfo(3000, 600, 40);
      SoundSources = new SoundSourceInfo[] { new SoundSourceInfo("engine_big", 2500.0f, new TV_3DVECTOR(0, 0, -800), true) };
      AddOns = new AddOnInfo[]
      {
        new AddOnInfo("Executor Super Star Destroyer Turbolaser Tower", new TV_3DVECTOR(500, 55, -450), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnInfo("Executor Super Star Destroyer Turbolaser Tower", new TV_3DVECTOR(-500, 55, -450), new TV_3DVECTOR(-90, 0, 0), true)

        , new AddOnInfo("Executor Super Star Destroyer Turbolaser Tower", new TV_3DVECTOR(500, 55, -700), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnInfo("Executor Super Star Destroyer Turbolaser Tower", new TV_3DVECTOR(-500, 55, -700), new TV_3DVECTOR(-90, 0, 0), true)

        , new AddOnInfo("Executor Super Star Destroyer Turbolaser Tower", new TV_3DVECTOR(200, 125, -600), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnInfo("Executor Super Star Destroyer Turbolaser Tower", new TV_3DVECTOR(-200, 125, -600), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnInfo("Executor Super Star Destroyer Turbolaser Tower", new TV_3DVECTOR(200, 125, -800), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnInfo("Executor Super Star Destroyer Turbolaser Tower", new TV_3DVECTOR(-200, 125, -800), new TV_3DVECTOR(-90, 0, 0), true)

        , new AddOnInfo("Executor Super Star Destroyer Turbolaser Tower", new TV_3DVECTOR(200, 140, -415), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnInfo("Executor Super Star Destroyer Turbolaser Tower", new TV_3DVECTOR(-200, 140, -415), new TV_3DVECTOR(-90, 0, 0), true)

        , new AddOnInfo("Executor Super Star Destroyer Turbolaser Tower", new TV_3DVECTOR(350, 150, -840), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnInfo("Executor Super Star Destroyer Turbolaser Tower", new TV_3DVECTOR(-350, 150, -840), new TV_3DVECTOR(-90, 0, 0), true)

        , new AddOnInfo("Executor Super Star Destroyer Turbolaser Tower", new TV_3DVECTOR(70, 125, -60), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnInfo("Executor Super Star Destroyer Turbolaser Tower", new TV_3DVECTOR(-70, 125, -60), new TV_3DVECTOR(-90, 0, 0), true)

        , new AddOnInfo("Executor Super Star Destroyer Turbolaser Tower", new TV_3DVECTOR(200, 130, 95), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnInfo("Executor Super Star Destroyer Turbolaser Tower", new TV_3DVECTOR(-200, 130, 95), new TV_3DVECTOR(-90, 0, 0), true)

        , new AddOnInfo("Executor Super Star Destroyer Turbolaser Tower", new TV_3DVECTOR(215, 70, 550), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnInfo("Executor Super Star Destroyer Turbolaser Tower", new TV_3DVECTOR(-215, 70, 550), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnInfo("Executor Super Star Destroyer Turbolaser Tower", new TV_3DVECTOR(215, 70, 675), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnInfo("Executor Super Star Destroyer Turbolaser Tower", new TV_3DVECTOR(-215, 70, 675), new TV_3DVECTOR(-90, 0, 0), true)

        , new AddOnInfo("Executor Super Star Destroyer Turbolaser Tower", new TV_3DVECTOR(340, 50, 410), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnInfo("Executor Super Star Destroyer Turbolaser Tower", new TV_3DVECTOR(-340, 50, 410), new TV_3DVECTOR(-90, 0, 0), true)

        , new AddOnInfo("Executor Super Star Destroyer Turbolaser Tower", new TV_3DVECTOR(75, 85, 1020), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnInfo("Executor Super Star Destroyer Turbolaser Tower", new TV_3DVECTOR(-75, 85, 1020), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnInfo("Executor Super Star Destroyer Turbolaser Tower", new TV_3DVECTOR(75, 70, 1160), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnInfo("Executor Super Star Destroyer Turbolaser Tower", new TV_3DVECTOR(-75, 70, 1160), new TV_3DVECTOR(-90, 0, 0), true)

        , new AddOnInfo("Executor Super Star Destroyer Turbolaser Tower", new TV_3DVECTOR(200, 60, 1000), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnInfo("Executor Super Star Destroyer Turbolaser Tower", new TV_3DVECTOR(-200, 60, 1000), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnInfo("Executor Super Star Destroyer Turbolaser Tower", new TV_3DVECTOR(200, 65, 1140), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnInfo("Executor Super Star Destroyer Turbolaser Tower", new TV_3DVECTOR(-200, 65, 1140), new TV_3DVECTOR(-90, 0, 0), true)

        , new AddOnInfo("Executor Super Star Destroyer Shield Generator", new TV_3DVECTOR(72, 375, -870), new TV_3DVECTOR(0, 0, 0), true)
        , new AddOnInfo("Executor Super Star Destroyer Shield Generator", new TV_3DVECTOR(-72, 375, -870), new TV_3DVECTOR(0, 0, 0), true)

        , new AddOnInfo("Executor Super Star Destroyer Bridge", new TV_3DVECTOR(0, 325, -845), new TV_3DVECTOR(0, 0, 0), true)
      };
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);
      ainfo.DyingMoveComponent = new DyingSink(0.00025f, 1.3f, 0.2f);
    }

    public override void ProcessNewState(ActorInfo ainfo)
    {
      base.ProcessNewState(ainfo);
      if (ainfo.ActorState.IsDying())
      {
        TimedLifeSystem.Activate(Engine, ainfo.ID, 2000f);
        CombatSystem.Deactivate(Engine, ainfo.ID);
      }
    }
  }
}

