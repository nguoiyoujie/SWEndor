using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes.Components;
using SWEndor.Models;
using SWEndor.Sound;

namespace SWEndor.ActorTypes.Instances
{
  internal class StrikeATI : Groups.Warship
  {
    internal StrikeATI(Factory owner) : base(owner, "STRKC", "Strike Cruiser")
    {
      // Combat
      Explodes = new ExplodeData[] {
        new ExplodeData("EXPL00", 0.5f, 1, ExplodeTrigger.ON_DYING | ExplodeTrigger.CREATE_ON_MESHVERTICES),
        new ExplodeData("EXPL01", 1, 1.5f, ExplodeTrigger.ON_DEATH),
        new ExplodeData("EXPW01", 1, 1, ExplodeTrigger.ON_DEATH)
      };

      SystemData.MaxShield = 350;
      SystemData.MaxHull = 420;
      CombatData.ImpactDamage = 60.0f;
      MoveLimitData.MaxSpeed = 105.0f;
      MoveLimitData.MinSpeed = 0.0f;
      MoveLimitData.MaxSpeedChangeRate = 8.0f;
      MoveLimitData.MaxTurnRate = 1.5f;

      RenderData.CullDistance = 35000;
      RegenData = new RegenData(false, 0.25f, 0, 0, 0);

      ScoreData = new ScoreData(50, 5000);

      MeshData = new MeshData(Engine, Name, @"strike\strike.x", 0.5f);

      Cameras = new LookData[] {
        new LookData(new TV_3DVECTOR(0, 145, -130), new TV_3DVECTOR(0, 145, 2000)),
        };
      DeathCamera = new DeathCameraData(1250, 200, 30);
      SoundSources = new SoundSourceData[] { new SoundSourceData(SoundGlobals.EngineShip, 500.0f, new TV_3DVECTOR(0, -60, -660), true, isEngineSound: true) };
      AddOns = new AddOnData[]
      {
        // Top
        new AddOnData("STRKLSR", new TV_3DVECTOR(0, 95, 365), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnData("STRKMPOD", new TV_3DVECTOR(0, 90, -245), new TV_3DVECTOR(-10, 0, 0), true)
        , new AddOnData("STRKLSR", new TV_3DVECTOR(0, 135, -550), new TV_3DVECTOR(-90, 0, 0), true)

        // Sides
        , new AddOnData("STRKLSR", new TV_3DVECTOR(60, 44, 520), new TV_3DVECTOR(-50, 75, 0), true)
        , new AddOnData("STRKLSR", new TV_3DVECTOR(-60, 44, 520), new TV_3DVECTOR(-50, -75, 0), true)
        , new AddOnData("STRKHLSR", new TV_3DVECTOR(90, 60, 90), new TV_3DVECTOR(-50, 90, 0), true)
        , new AddOnData("STRKHLSR", new TV_3DVECTOR(-90, 60, 90), new TV_3DVECTOR(-50, -90, 0), true)

        , new AddOnData("STRKLSR", new TV_3DVECTOR(80, 48, -145), new TV_3DVECTOR(-50, 90, 0), true)
        , new AddOnData("STRKLSR", new TV_3DVECTOR(-80, 48, -145), new TV_3DVECTOR(-50, -90, 0), true)

        // Bottom
        , new AddOnData("STRKHLSR", new TV_3DVECTOR(125, -55, 200), new TV_3DVECTOR(90, 0, 0), true)
        , new AddOnData("STRKHLSR", new TV_3DVECTOR(-125, -55, 200), new TV_3DVECTOR(90, 0, 0), true)
        , new AddOnData("STRKHLSR", new TV_3DVECTOR(125, -55, 20), new TV_3DVECTOR(90, 0, 0), true)
        , new AddOnData("STRKHLSR", new TV_3DVECTOR(-125, -55, 20), new TV_3DVECTOR(90, 0, 0), true)

        , new AddOnData("STRKLSR", new TV_3DVECTOR(0, -125, -550), new TV_3DVECTOR(90, 0, 0), true)
      };
    }
  }
}

