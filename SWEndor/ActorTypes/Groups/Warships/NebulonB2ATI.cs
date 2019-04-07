using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes.Components;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.ActorTypes
{
  public class NebulonB2ATI : WarshipGroup
  {
    private static NebulonB2ATI _instance;
    public static NebulonB2ATI Instance()
    {
      if (_instance == null) { _instance = new NebulonB2ATI(); }
      return _instance;
    }

    private NebulonB2ATI() : base("Nebulon-B2 Frigate")
    {
      MaxStrength = 1350.0f;
      ImpactDamage = 60.0f;
      MaxSpeed = 36.0f;
      MinSpeed = 0.0f;
      MaxSpeedChangeRate = 10.0f;
      MaxTurnRate = 2.5f;

      CullDistance = 30000;

      Score_perStrength = 15;
      Score_DestroyBonus = 10000;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"nebulonb\nebulonb2.x");

      SoundSources = new SoundSourceInfo[] { new SoundSourceInfo("engine_big", new TV_3DVECTOR(0, 100, -300), 1500.0f, true) };
      AddOns = new AddOnInfo[]
      {
        new AddOnInfo("Nebulon B Turbolaser Tower", new TV_3DVECTOR(0, 40, 220), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnInfo("Nebulon B Turbolaser Tower", new TV_3DVECTOR(0, 95, -520), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnInfo("Nebulon B Turbolaser Tower", new TV_3DVECTOR(0, -145, -550), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnInfo("Nebulon B Missile Pod", new TV_3DVECTOR(-80, -45, -485), new TV_3DVECTOR(0, 0, 0), true)
        , new AddOnInfo("Nebulon B Missile Pod", new TV_3DVECTOR(80, -45, -485), new TV_3DVECTOR(0, 0, 0), true)
      };
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.MovementInfo.DyingMovement = Actors.Components.DyingMovement.SINK;
      ainfo.MovementInfo.D_sink_pitch_rate = 0.02f;
      ainfo.MovementInfo.D_sink_down_rate = 5f;
      ainfo.MovementInfo.D_sink_forward_rate = 0.8f;

      ainfo.RegenerationInfo.SelfRegenRate = 0.1f;

      ainfo.CamLocations.Add(new TV_3DVECTOR(0, 120, -300));
      ainfo.CamTargets.Add(new TV_3DVECTOR(0, 120, 2000));
    }
  }
}

