using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class MC80BATI : Groups.Warship
  {
    internal MC80BATI(Factory owner) : base(owner, "Mon Calamari 80B Capital Ship")
    {
      Explodes = new ExplodeInfo[] {
        new ExplodeInfo("ExpL00", 0.5f, 1, ExplodeTrigger.ON_DYING | ExplodeTrigger.CREATE_ON_MESHVERTICES),
        new ExplodeInfo("ExpL01", 1, 3.5f, ExplodeTrigger.ON_DEATH),
        new ExplodeInfo("ExpW01", 1, 1, ExplodeTrigger.ON_DEATH)
      };

      MaxStrength = 2800.0f;
      ImpactDamage = 60.0f;
      MoveLimitData.MaxSpeed = 30.0f;
      MoveLimitData.MinSpeed = 0.0f;
      MoveLimitData.MaxSpeedChangeRate = 5.0f;
      MoveLimitData.MaxTurnRate = 3f;
      MoveLimitData.ZNormFrac = 0.006f;
      MoveLimitData.ZTilt = 3.5f;

      Scale = 1.8f;

      ScoreData = new ScoreData(20, 20000);
      RenderData.CullDistance = 40000;

      RegenData = new RegenData(false, 0.3f, 0, 0, 0);

      MeshData = new MeshData(Name, @"mc90\mc80b.x");

      Cameras = new LookData[] { new LookData(new TV_3DVECTOR(0, 45, 660), new TV_3DVECTOR(0, 45, 2000)) };
      DeathCamera = new DeathCameraData(1500, 250, 30);
      SoundSources = new SoundSourceData[] { new SoundSourceData("engine_big", 1500f, new TV_3DVECTOR(0, 0, -750), true) };
      AddOns = new AddOnData[]
      {
        new AddOnData("MC90 Turbolaser Tower", new TV_3DVECTOR(0, 45, 1200), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnData("MC90 Turbolaser Tower", new TV_3DVECTOR(-120, 42, 950), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnData("MC90 Turbolaser Tower", new TV_3DVECTOR(120, 42, 950), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnData("MC90 Turbolaser Tower", new TV_3DVECTOR(-180, 48, 520), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnData("MC90 Turbolaser Tower", new TV_3DVECTOR(180, 48, 520), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnData("MC90 Turbolaser Tower", new TV_3DVECTOR(-180, -65, 410), new TV_3DVECTOR(90, 0, 0), true)
        , new AddOnData("MC90 Turbolaser Tower", new TV_3DVECTOR(180, -65, 410), new TV_3DVECTOR(90, 0, 0), true)

        , new AddOnData("MC90 Turbolaser Tower", new TV_3DVECTOR(-220, 52, 300), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnData("MC90 Turbolaser Tower", new TV_3DVECTOR(220, 52, 300), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnData("MC90 Turbolaser Tower", new TV_3DVECTOR(-210, -75, 150), new TV_3DVECTOR(90, 0, 0), true)
        , new AddOnData("MC90 Turbolaser Tower", new TV_3DVECTOR(210, -75, 150), new TV_3DVECTOR(90, 0, 0), true)

        , new AddOnData("MC90 Turbolaser Tower", new TV_3DVECTOR(0, 120, -225), new TV_3DVECTOR(-90, 0, 0), true)
      };
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.DyingMoveComponent = new DyingSink(0.01f, 2.5f, 0.4f);
    }
  }
}

