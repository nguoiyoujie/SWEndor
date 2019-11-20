using MTV3D65;
using SWEndor.ActorTypes.Components;
using SWEndor.Models;
using SWEndor.Sound;

namespace SWEndor.ActorTypes.Instances
{
  internal class ExecutorATI : Groups.StarDestroyer
  {
    internal ExecutorATI(Factory owner) : base(owner, "EXEC", "EXEC")
    {
      Explodes = new ExplodeData[]
      {
        new ExplodeData("EXPL00", 0.5f, 1, ExplodeTrigger.ON_DYING | ExplodeTrigger.CREATE_ON_MESHVERTICES),
        new ExplodeData("EXPL01", 1, 5, ExplodeTrigger.ON_DEATH),
        new ExplodeData("EXPW01", 1, 5, ExplodeTrigger.ON_DEATH)
      };

      SystemData.MaxShield = 2250;
      SystemData.MaxHull = 2250;
      CombatData.ImpactDamage = 120.0f;
      MoveLimitData.MaxSpeed = 30;
      MoveLimitData.MinSpeed = 0.0f;
      MoveLimitData.MaxSpeedChangeRate = 2.0f;
      MoveLimitData.MaxTurnRate = 2f;
      RegenData = new RegenData(false, 1, 0, 0, 0);

      RenderData.CullDistance = 60000;
      TimedLifeData = new TimedLifeData(false, 2000);
      ScoreData = new ScoreData(75, 100000);

      MeshData = new MeshData(Engine, Name, @"executor\executor.x", 2.5f);
      DyingMoveData.Sink(0.00025f, 1.3f, 0.2f);

      Cameras = new LookData[] {
        new LookData(new TV_3DVECTOR(0, 325, -840), new TV_3DVECTOR(0, 325, 2000)),
        };
      DeathCamera = new DeathCameraData(3000, 600, 40);
      SoundSources = new SoundSourceData[] { new SoundSourceData(SoundGlobals.EngineShip, 2500.0f, new TV_3DVECTOR(0, 0, -800), true, isEngineSound: true) };
      AddOns = new AddOnData[]
      {
        new AddOnData("EXECLSR", new TV_3DVECTOR(500, 55, -450), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnData("EXECLSR", new TV_3DVECTOR(-500, 55, -450), new TV_3DVECTOR(-90, 0, 0), true)

        , new AddOnData("EXECLSR", new TV_3DVECTOR(500, 55, -700), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnData("EXECLSR", new TV_3DVECTOR(-500, 55, -700), new TV_3DVECTOR(-90, 0, 0), true)

        , new AddOnData("EXECLSR", new TV_3DVECTOR(200, 125, -600), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnData("EXECLSR", new TV_3DVECTOR(-200, 125, -600), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnData("EXECLSR", new TV_3DVECTOR(200, 125, -800), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnData("EXECLSR", new TV_3DVECTOR(-200, 125, -800), new TV_3DVECTOR(-90, 0, 0), true)

        , new AddOnData("EXECLSR", new TV_3DVECTOR(200, 140, -415), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnData("EXECLSR", new TV_3DVECTOR(-200, 140, -415), new TV_3DVECTOR(-90, 0, 0), true)

        , new AddOnData("EXECLSR", new TV_3DVECTOR(350, 150, -840), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnData("EXECLSR", new TV_3DVECTOR(-350, 150, -840), new TV_3DVECTOR(-90, 0, 0), true)

        , new AddOnData("EXECLSR", new TV_3DVECTOR(70, 125, -60), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnData("EXECLSR", new TV_3DVECTOR(-70, 125, -60), new TV_3DVECTOR(-90, 0, 0), true)

        , new AddOnData("EXECLSR", new TV_3DVECTOR(200, 130, 95), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnData("EXECLSR", new TV_3DVECTOR(-200, 130, 95), new TV_3DVECTOR(-90, 0, 0), true)

        , new AddOnData("EXECLSR", new TV_3DVECTOR(215, 70, 550), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnData("EXECLSR", new TV_3DVECTOR(-215, 70, 550), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnData("EXECLSR", new TV_3DVECTOR(215, 70, 675), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnData("EXECLSR", new TV_3DVECTOR(-215, 70, 675), new TV_3DVECTOR(-90, 0, 0), true)

        , new AddOnData("EXECLSR", new TV_3DVECTOR(340, 50, 410), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnData("EXECLSR", new TV_3DVECTOR(-340, 50, 410), new TV_3DVECTOR(-90, 0, 0), true)

        , new AddOnData("EXECLSR", new TV_3DVECTOR(75, 85, 1020), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnData("EXECLSR", new TV_3DVECTOR(-75, 85, 1020), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnData("EXECLSR", new TV_3DVECTOR(75, 70, 1160), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnData("EXECLSR", new TV_3DVECTOR(-75, 70, 1160), new TV_3DVECTOR(-90, 0, 0), true)

        , new AddOnData("EXECLSR", new TV_3DVECTOR(200, 60, 1000), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnData("EXECLSR", new TV_3DVECTOR(-200, 60, 1000), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnData("EXECLSR", new TV_3DVECTOR(200, 65, 1140), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnData("EXECLSR", new TV_3DVECTOR(-200, 65, 1140), new TV_3DVECTOR(-90, 0, 0), true)

        , new AddOnData("EXECSHD", new TV_3DVECTOR(72, 375, -870), new TV_3DVECTOR(0, 0, 0), true)
        , new AddOnData("EXECSHD", new TV_3DVECTOR(-72, 375, -870), new TV_3DVECTOR(0, 0, 0), true)

        , new AddOnData("EXERBRID", new TV_3DVECTOR(0, 325, -845), new TV_3DVECTOR(0, 0, 0), true)
      };
    }
  }
}

