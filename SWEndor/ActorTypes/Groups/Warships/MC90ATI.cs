using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.ActorTypes.Components;
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
      MaxSpeed = 30.0f;
      MinSpeed = 0.0f;
      MaxSpeedChangeRate = 5.0f;
      MaxTurnRate = 2.6f;
      ZNormFrac = 0.006f;
      ZTilt = 2.2f; // low number so fighters come out properly when mothership is rotating

      CullDistance = 30000;
      Scale = 1.8f;

      Score_perStrength = 20;
      Score_DestroyBonus = 20000;

      RegenData = new RegenInfo { SelfRegenRate = 0.35f };

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"mc90\mc90.x");
      SourceFarMeshPath = Path.Combine(Globals.ModelPath, @"mc90\mc90_far.x");

      Cameras = new ActorCameraInfo[] { new ActorCameraInfo(new TV_3DVECTOR(0, 45, 660), new TV_3DVECTOR(0, 45, 2000)) };
      DeathCamera = new DeathCameraInfo(1500, 250, 30);
      SoundSources = new SoundSourceInfo[] { new SoundSourceInfo("engine_big", 1500f, new TV_3DVECTOR(0, 0, -750), true) };
      AddOns = new AddOnInfo[]
      {
        new AddOnInfo("MC90 Turbolaser Tower", new TV_3DVECTOR(100, 75, 500), new TV_3DVECTOR(-90, 0, -10), true)
        , new AddOnInfo("MC90 Turbolaser Tower", new TV_3DVECTOR(-100, 75, 500), new TV_3DVECTOR(-90, 0, 10), true)
        , new AddOnInfo("MC90 Turbolaser Tower", new TV_3DVECTOR(0, 135, 0), new TV_3DVECTOR(-90, 0, 0), true)

        //bottom
        , new AddOnInfo("MC90 Turbolaser Tower", new TV_3DVECTOR(0, -40, 700), new TV_3DVECTOR(90, 0, 0), true)
        , new AddOnInfo("MC90 Turbolaser Tower", new TV_3DVECTOR(0, 40, 700), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnInfo("MC90 Turbolaser Tower", new TV_3DVECTOR(170, 40, 100), new TV_3DVECTOR(-90, 0, -15), true)
        , new AddOnInfo("MC90 Turbolaser Tower", new TV_3DVECTOR(-170, 40, 100), new TV_3DVECTOR(-90, 0, 15), true)
        , new AddOnInfo("MC90 Turbolaser Tower", new TV_3DVECTOR(210, 40, -300), new TV_3DVECTOR(-90, 0, -20), true)
        , new AddOnInfo("MC90 Turbolaser Tower", new TV_3DVECTOR(-210, 40, -300), new TV_3DVECTOR(-90, 0, 20), true)
        , new AddOnInfo("MC90 Turbolaser Tower", new TV_3DVECTOR(170, 40, -600), new TV_3DVECTOR(-90, 0, -15), true)
        , new AddOnInfo("MC90 Turbolaser Tower", new TV_3DVECTOR(-170, 40, -600), new TV_3DVECTOR(-90, 0, 15), true)

        // Hangar Bay
        , new AddOnInfo("Hangar Bay", new TV_3DVECTOR(95, -30, -200), new TV_3DVECTOR(0, 0, 0), true)
        , new AddOnInfo("Hangar Bay", new TV_3DVECTOR(-145, -25, -27), new TV_3DVECTOR(0, 180, 0), true)
        , new AddOnInfo("Hangar Bay", new TV_3DVECTOR(-145, -25, -230), new TV_3DVECTOR(0, 180, 0), true)
      };
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.DyingMoveComponent = new DyingSink(0.01f, 2.5f, 0.4f);
      ainfo.SpawnerInfo = new MC90Spawner(ainfo);
    }
  }
}

