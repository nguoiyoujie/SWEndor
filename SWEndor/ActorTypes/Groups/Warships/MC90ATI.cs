using MTV3D65;
using SWEndor.Actors;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.ActorTypes
{
  public class MC90ATI : WarshipGroup
  {
    private static MC90ATI _instance;
    public static MC90ATI Instance()
    {
      if (_instance == null) { _instance = new MC90ATI(); }
      return _instance;
    }

    private MC90ATI() : base("Mon Calamari Capital Ship")
    {
      MaxStrength = 3200.0f;
      ImpactDamage = 60.0f;
      MaxSpeed = 30.0f;
      MinSpeed = 0.0f;
      MaxSpeedChangeRate = 5.0f;
      MaxTurnRate = 3f;
      ZNormFrac = 0.006f;
      ZTilt = 1.8f; // low number so fighters come out properly when mothership is rotating

      Score_perStrength = 20;
      Score_DestroyBonus = 20000;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"mc90\mc90.x");

      SoundSources = new SoundSourceInfo[] { new SoundSourceInfo("engine_big", new TV_3DVECTOR(0, 0, -750), 1500.0f, true) };
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
      };
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.CamDeathCircleRadius = 1500;
      ainfo.CamDeathCircleHeight = 250;
      ainfo.CamDeathCirclePeriod = 30;

      ainfo.CamLocations.Add(new TV_3DVECTOR(0, 45, 660));
      ainfo.CamTargets.Add(new TV_3DVECTOR(0, 45, 2000));

      ainfo.ExplosionInfo.DeathExplosionSize = 3.5f;

      ainfo.MovementInfo.DyingMovement = Actors.Components.DyingMovement.SINK;
      ainfo.MovementInfo.D_sink_pitch_rate = 0.01f;
      ainfo.MovementInfo.D_sink_down_rate = 2.5f;
      ainfo.MovementInfo.D_sink_forward_rate = 0.4f;

      ainfo.RegenerationInfo.SelfRegenRate = 0.2f;

      ainfo.Scale *= 1.1f;

      ainfo.SpawnerInfo = new MC90Spawner(ainfo);
    }
  }
}

