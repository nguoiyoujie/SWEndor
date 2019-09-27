using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Actors.Models;
using SWEndor.ActorTypes.Components;
using SWEndor.Sound;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class ExecutorATI : Groups.StarDestroyer
  {
    internal ExecutorATI(Factory owner) : base(owner, "Executor Super Star Destroyer")
    {
      Explodes = new ExplodeInfo[]
      {
        new ExplodeInfo("ExpL00", 0.5f, 1, ExplodeTrigger.ON_DYING | ExplodeTrigger.CREATE_ON_MESHVERTICES),
        new ExplodeInfo("ExpL01", 1, 5, ExplodeTrigger.ON_DEATH),
        new ExplodeInfo("ExpW01", 1, 5, ExplodeTrigger.ON_DEATH)
      };

      MaxStrength = 3500.0f;
      ImpactDamage = 120.0f;
      MoveLimitData.MaxSpeed = 30;
      MoveLimitData.MinSpeed = 0.0f;
      MoveLimitData.MaxSpeedChangeRate = 2.0f;
      MoveLimitData.MaxTurnRate = 2f;

      RenderData.CullDistance = 60000;
      Scale = 2.5f;

      ScoreData = new ScoreData(75, 100000);

      MeshData = new MeshData(Name, @"executor\executor.x");
      DyingMoveData.Sink(0.00025f, 1.3f, 0.2f);

      Cameras = new LookData[] {
        new LookData(new TV_3DVECTOR(0, 325, -840), new TV_3DVECTOR(0, 325, 2000)),
        };
      DeathCamera = new DeathCameraData(3000, 600, 40);
      SoundSources = new SoundSourceData[] { new SoundSourceData(SoundGlobals.EngineShip, 2500.0f, new TV_3DVECTOR(0, 0, -800), true) };
      AddOns = new AddOnData[]
      {
        new AddOnData("Executor Super Star Destroyer Turbolaser Tower", new TV_3DVECTOR(500, 55, -450), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnData("Executor Super Star Destroyer Turbolaser Tower", new TV_3DVECTOR(-500, 55, -450), new TV_3DVECTOR(-90, 0, 0), true)

        , new AddOnData("Executor Super Star Destroyer Turbolaser Tower", new TV_3DVECTOR(500, 55, -700), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnData("Executor Super Star Destroyer Turbolaser Tower", new TV_3DVECTOR(-500, 55, -700), new TV_3DVECTOR(-90, 0, 0), true)

        , new AddOnData("Executor Super Star Destroyer Turbolaser Tower", new TV_3DVECTOR(200, 125, -600), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnData("Executor Super Star Destroyer Turbolaser Tower", new TV_3DVECTOR(-200, 125, -600), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnData("Executor Super Star Destroyer Turbolaser Tower", new TV_3DVECTOR(200, 125, -800), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnData("Executor Super Star Destroyer Turbolaser Tower", new TV_3DVECTOR(-200, 125, -800), new TV_3DVECTOR(-90, 0, 0), true)

        , new AddOnData("Executor Super Star Destroyer Turbolaser Tower", new TV_3DVECTOR(200, 140, -415), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnData("Executor Super Star Destroyer Turbolaser Tower", new TV_3DVECTOR(-200, 140, -415), new TV_3DVECTOR(-90, 0, 0), true)

        , new AddOnData("Executor Super Star Destroyer Turbolaser Tower", new TV_3DVECTOR(350, 150, -840), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnData("Executor Super Star Destroyer Turbolaser Tower", new TV_3DVECTOR(-350, 150, -840), new TV_3DVECTOR(-90, 0, 0), true)

        , new AddOnData("Executor Super Star Destroyer Turbolaser Tower", new TV_3DVECTOR(70, 125, -60), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnData("Executor Super Star Destroyer Turbolaser Tower", new TV_3DVECTOR(-70, 125, -60), new TV_3DVECTOR(-90, 0, 0), true)

        , new AddOnData("Executor Super Star Destroyer Turbolaser Tower", new TV_3DVECTOR(200, 130, 95), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnData("Executor Super Star Destroyer Turbolaser Tower", new TV_3DVECTOR(-200, 130, 95), new TV_3DVECTOR(-90, 0, 0), true)

        , new AddOnData("Executor Super Star Destroyer Turbolaser Tower", new TV_3DVECTOR(215, 70, 550), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnData("Executor Super Star Destroyer Turbolaser Tower", new TV_3DVECTOR(-215, 70, 550), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnData("Executor Super Star Destroyer Turbolaser Tower", new TV_3DVECTOR(215, 70, 675), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnData("Executor Super Star Destroyer Turbolaser Tower", new TV_3DVECTOR(-215, 70, 675), new TV_3DVECTOR(-90, 0, 0), true)

        , new AddOnData("Executor Super Star Destroyer Turbolaser Tower", new TV_3DVECTOR(340, 50, 410), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnData("Executor Super Star Destroyer Turbolaser Tower", new TV_3DVECTOR(-340, 50, 410), new TV_3DVECTOR(-90, 0, 0), true)

        , new AddOnData("Executor Super Star Destroyer Turbolaser Tower", new TV_3DVECTOR(75, 85, 1020), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnData("Executor Super Star Destroyer Turbolaser Tower", new TV_3DVECTOR(-75, 85, 1020), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnData("Executor Super Star Destroyer Turbolaser Tower", new TV_3DVECTOR(75, 70, 1160), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnData("Executor Super Star Destroyer Turbolaser Tower", new TV_3DVECTOR(-75, 70, 1160), new TV_3DVECTOR(-90, 0, 0), true)

        , new AddOnData("Executor Super Star Destroyer Turbolaser Tower", new TV_3DVECTOR(200, 60, 1000), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnData("Executor Super Star Destroyer Turbolaser Tower", new TV_3DVECTOR(-200, 60, 1000), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnData("Executor Super Star Destroyer Turbolaser Tower", new TV_3DVECTOR(200, 65, 1140), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnData("Executor Super Star Destroyer Turbolaser Tower", new TV_3DVECTOR(-200, 65, 1140), new TV_3DVECTOR(-90, 0, 0), true)

        , new AddOnData("Executor Super Star Destroyer Shield Generator", new TV_3DVECTOR(72, 375, -870), new TV_3DVECTOR(0, 0, 0), true)
        , new AddOnData("Executor Super Star Destroyer Shield Generator", new TV_3DVECTOR(-72, 375, -870), new TV_3DVECTOR(0, 0, 0), true)

        , new AddOnData("Executor Super Star Destroyer Bridge", new TV_3DVECTOR(0, 325, -845), new TV_3DVECTOR(0, 0, 0), true)
      };
    }

    public override void Dying(ActorInfo ainfo)
    {
      ainfo.DyingTimerSet(2000, true);
    }
  }
}

