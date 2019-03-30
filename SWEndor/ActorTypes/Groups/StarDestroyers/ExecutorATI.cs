using MTV3D65;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.Actors.Types
{
  public class ExecutorATI : StarDestroyerGroup
  {
    private static ExecutorATI _instance;
    public static ExecutorATI Instance()
    {
      if (_instance == null) { _instance = new ExecutorATI(); }
      return _instance;
    }

    private ExecutorATI() : base("Executor Super Star Destroyer")
    {
      MaxStrength = 3500.0f;
      ImpactDamage = 120.0f;
      MaxSpeed = 30;
      MinSpeed = 0.0f;
      MaxSpeedChangeRate = 2.0f;
      MaxTurnRate = 2f;

      Score_perStrength = 75;
      Score_DestroyBonus = 100000;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"executor\executor.x");

      SoundSources = new SoundSourceInfo[] { new SoundSourceInfo("engine_big", new TV_3DVECTOR(0, 0, -800), 2500.0f, true) };
      AddOns = new AddOnInfo[]
      {
        new AddOnInfo("Executor Super Star Destroyer Turbolaser Tower", new TV_3DVECTOR(500, 55, -450), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnInfo("Executor Super Star Destroyer Turbolaser Tower", new TV_3DVECTOR(-500, 55, -450), new TV_3DVECTOR(-90, 0, 0), true)

        , new AddOnInfo("Executor Super Star Destroyer Turbolaser Tower", new TV_3DVECTOR(570, 55, -700), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnInfo("Executor Super Star Destroyer Turbolaser Tower", new TV_3DVECTOR(-570, 55, -700), new TV_3DVECTOR(-90, 0, 0), true)

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

      ainfo.CamDeathCircleRadius = 3000;
      ainfo.CamDeathCircleHeight = 600;
      ainfo.CamDeathCirclePeriod = 40;

      // Camera System
      ainfo.CamLocations.Add(new TV_3DVECTOR(0, 325, -840));
      ainfo.CamTargets.Add(new TV_3DVECTOR(0, 325, 2000));
      ainfo.CamLocations.Add(new TV_3DVECTOR(0, 1000, -2000));
      ainfo.CamTargets.Add(new TV_3DVECTOR(0, 325, 2000));

      ainfo.ExplosionInfo.DeathExplosionSize = 5;

      ainfo.MovementInfo.DyingMovement = Components.DyingMovement.SINK;
      ainfo.MovementInfo.D_sink_pitch_rate = 0.00025f;
      ainfo.MovementInfo.D_sink_down_rate = 1.3f;
      ainfo.MovementInfo.D_sink_forward_rate = 0.2f;

      ainfo.Scale *= 2.5f;
    }

    public override void ProcessNewState(ActorInfo ainfo)
    {
      base.ProcessNewState(ainfo);
      if (ainfo.ActorState == ActorState.DYING)
      {
        ainfo.CombatInfo.OnTimedLife = true;
        ainfo.CombatInfo.TimedLife = 2000f;
        ainfo.CombatInfo.IsCombatObject = false;
      }
    }
  }
}

