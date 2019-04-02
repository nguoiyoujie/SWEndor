using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes.Components;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.ActorTypes
{
  public class MC80BATI : WarshipGroup
  {
    private static MC80BATI _instance;
    public static MC80BATI Instance()
    {
      if (_instance == null) { _instance = new MC80BATI(); }
      return _instance;
    }

    private MC80BATI() : base("Mon Calamari 80B Capital Ship")
    {
      MaxStrength = 2800.0f;
      ImpactDamage = 60.0f;
      MaxSpeed = 30.0f;
      MinSpeed = 0.0f;
      MaxSpeedChangeRate = 5.0f;
      MaxTurnRate = 3f;
      ZNormFrac = 0.006f;
      ZTilt = 3.5f;

      Score_perStrength = 20;
      Score_DestroyBonus = 20000;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"mc90\mc80b.x");

      SoundSources = new SoundSourceInfo[] { new SoundSourceInfo("engine_big", new TV_3DVECTOR(0, 0, -750), 1500.0f, true) };
      AddOns = new AddOnInfo[]
      {
        new AddOnInfo("MC90 Turbolaser Tower", new TV_3DVECTOR(0, 45, 1200), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnInfo("MC90 Turbolaser Tower", new TV_3DVECTOR(-120, 42, 950), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnInfo("MC90 Turbolaser Tower", new TV_3DVECTOR(120, 42, 950), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnInfo("MC90 Turbolaser Tower", new TV_3DVECTOR(-180, 48, 520), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnInfo("MC90 Turbolaser Tower", new TV_3DVECTOR(180, 48, 520), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnInfo("MC90 Turbolaser Tower", new TV_3DVECTOR(-180, -65, 410), new TV_3DVECTOR(90, 0, 0), true)
        , new AddOnInfo("MC90 Turbolaser Tower", new TV_3DVECTOR(180, -65, 410), new TV_3DVECTOR(90, 0, 0), true)

        , new AddOnInfo("MC90 Turbolaser Tower", new TV_3DVECTOR(-220, 52, 300), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnInfo("MC90 Turbolaser Tower", new TV_3DVECTOR(220, 52, 300), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnInfo("MC90 Turbolaser Tower", new TV_3DVECTOR(-210, -75, 150), new TV_3DVECTOR(90, 0, 0), true)
        , new AddOnInfo("MC90 Turbolaser Tower", new TV_3DVECTOR(210, -75, 150), new TV_3DVECTOR(90, 0, 0), true)

        , new AddOnInfo("MC90 Turbolaser Tower", new TV_3DVECTOR(0, 120, -225), new TV_3DVECTOR(-90, 0, 0), true)
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
    }
  }
}

