using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes.Components;
using SWEndor.Models;
using SWEndor.Sound;

namespace SWEndor.ActorTypes.Instances
{
  internal class MCLATI : Groups.Warship
  {
    internal MCLATI(Factory owner) : base(owner, "MCL", "Mon Calamari Light Cruiser")
    {
      Explodes = new ExplodeData[] {
        new ExplodeData("EXPL00", 0.5f, 1, ExplodeTrigger.ON_DYING | ExplodeTrigger.CREATE_ON_MESHVERTICES),
        new ExplodeData("EXPL01", 1, 3.5f, ExplodeTrigger.ON_DEATH),
        new ExplodeData("EXPW01", 1, 1, ExplodeTrigger.ON_DEATH)
      };

      SystemData.MaxShield = 1000;
      SystemData.MaxHull = 400;
      CombatData.ImpactDamage = 60.0f;
      MoveLimitData.MaxSpeed = 45.0f;
      MoveLimitData.MinSpeed = 0.0f;
      MoveLimitData.MaxSpeedChangeRate = 10.0f;
      MoveLimitData.MaxTurnRate = 6f;
      MoveLimitData.ZNormFrac = 0.006f;
      MoveLimitData.ZTilt = 1.8f; // low number so fighters come out properly when mothership is rotating

      RenderData.CullDistance = 45000;
      ScoreData = new ScoreData(20, 20000);

      RegenData = new RegenData(false, 2.5f, 0, 0, 0);

      MeshData = new MeshData(Engine, Name, @"mc90\mclc.x", 1.1f);
      DyingMoveData.Sink(0.01f, 2.5f, 0.4f);

      Cameras = new LookData[] { new LookData(new TV_3DVECTOR(0, 45, 660), new TV_3DVECTOR(0, 45, 2000)) };
      DeathCamera = new DeathCameraData(1500, 250, 30);
      SoundSources = new SoundSourceData[] { new SoundSourceData(SoundGlobals.EngineShip, 1500f, new TV_3DVECTOR(0, 0, -750), true, isEngineSound: true) };
      AddOns = new AddOnData[]
      {
        new AddOnData("MC90LSR", new TV_3DVECTOR(0, 140, -630), new TV_3DVECTOR(-90, 0, 0), true)

        , new AddOnData("MC90LSR", new TV_3DVECTOR(-70, 32, 450), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnData("MC90LSR", new TV_3DVECTOR(70, 42, 450), new TV_3DVECTOR(-90, 0, 0), true)

        , new AddOnData("MC90LSR", new TV_3DVECTOR(-100, 35, 230), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnData("MC90LSR", new TV_3DVECTOR(100, 35, 230), new TV_3DVECTOR(-90, 0, 0), true)

        , new AddOnData("MC90LSR", new TV_3DVECTOR(-90, -85, 175), new TV_3DVECTOR(90, 0, 0), true)
        , new AddOnData("MC90LSR", new TV_3DVECTOR(90, -85, 175), new TV_3DVECTOR(90, 0, 0), true)

        , new AddOnData("MC90LSR", new TV_3DVECTOR(-80, 80, -335), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnData("MC90LSR", new TV_3DVECTOR(80, 80, -335), new TV_3DVECTOR(-90, 0, 0), true)

        // Hangar Bay
        , new AddOnData("HANGAR", new TV_3DVECTOR(95, -30, -200), new TV_3DVECTOR(0, 0, 0), true)
        , new AddOnData("HANGAR", new TV_3DVECTOR(-95, -30, -200), new TV_3DVECTOR(0, 180, 0), true)
      };
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);
      ainfo.SpawnerInfo = SpawnerInfoDecorator.MCL_Default;
    }
  }
}

