using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Actors.Data;
using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class MCLATI : Groups.Warship
  {
    internal MCLATI(Factory owner) : base(owner, "Mon Calamari Light Cruiser")
    {
      ExplodeData = new ExplodeData(0.5f, 1, "ExplosionSm", DeathExplosionTrigger.ALWAYS, 3.5f, "ExplosionLg");

      MaxStrength = 1400.0f;
      ImpactDamage = 60.0f;
      MaxSpeed = 45.0f;
      MinSpeed = 0.0f;
      MaxSpeedChangeRate = 10.0f;
      MaxTurnRate = 6f;
      ZNormFrac = 0.006f;
      ZTilt = 1.8f; // low number so fighters come out properly when mothership is rotating

      CullDistance = 25000;
      Scale = 1.1f;

      Score_perStrength = 20;
      Score_DestroyBonus = 20000;

      RegenData = new RegenData { SelfRegenRate = 0.35f };

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"mc90\mclc.x");

      SoundSources = new SoundSourceInfo[] { new SoundSourceInfo("engine_big", 1500f, new TV_3DVECTOR(0, 0, -750), true) };
      AddOns = new AddOnInfo[]
      {
        new AddOnInfo("MC90 Turbolaser Tower", new TV_3DVECTOR(0, 140, -630), new TV_3DVECTOR(-90, 0, 0), true)

        , new AddOnInfo("MC90 Turbolaser Tower", new TV_3DVECTOR(-70, 32, 450), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnInfo("MC90 Turbolaser Tower", new TV_3DVECTOR(70, 42, 450), new TV_3DVECTOR(-90, 0, 0), true)

        , new AddOnInfo("MC90 Turbolaser Tower", new TV_3DVECTOR(-100, 35, 230), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnInfo("MC90 Turbolaser Tower", new TV_3DVECTOR(100, 35, 230), new TV_3DVECTOR(-90, 0, 0), true)

        , new AddOnInfo("MC90 Turbolaser Tower", new TV_3DVECTOR(-90, -85, 175), new TV_3DVECTOR(90, 0, 0), true)
        , new AddOnInfo("MC90 Turbolaser Tower", new TV_3DVECTOR(90, -85, 175), new TV_3DVECTOR(90, 0, 0), true)

        , new AddOnInfo("MC90 Turbolaser Tower", new TV_3DVECTOR(-80, 80, -335), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnInfo("MC90 Turbolaser Tower", new TV_3DVECTOR(80, 80, -335), new TV_3DVECTOR(-90, 0, 0), true)

        // Hangar Bay
        , new AddOnInfo("Hangar Bay", new TV_3DVECTOR(95, -30, -200), new TV_3DVECTOR(0, 0, 0), true)
        , new AddOnInfo("Hangar Bay", new TV_3DVECTOR(-95, -30, -200), new TV_3DVECTOR(0, 180, 0), true)
      };
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.CameraSystemInfo.CamDeathCircleRadius = 1500;
      ainfo.CameraSystemInfo.CamDeathCircleHeight = 250;
      ainfo.CameraSystemInfo.CamDeathCirclePeriod = 30;

      ainfo.CameraSystemInfo.CamLocations = new TV_3DVECTOR[] { new TV_3DVECTOR(0, 45, 660) };
      ainfo.CameraSystemInfo.CamTargets = new TV_3DVECTOR[] { new TV_3DVECTOR(0, 45, 2000) };

      ainfo.DyingMoveComponent = new DyingSink(0.01f, 2.5f, 0.4f);

      ainfo.SpawnerInfo = new MCLSpawner(ainfo);
    }
  }
}

