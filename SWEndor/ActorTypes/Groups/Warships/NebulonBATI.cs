using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class NebulonBATI : Groups.Warship
  {
    private static NebulonBATI _instance;
    public static NebulonBATI Instance()
    {
      if (_instance == null) { _instance = new NebulonBATI(); }
      return _instance;
    }

    private NebulonBATI() : base("Nebulon-B Frigate")
    {
      MaxStrength = 950.0f;
      ImpactDamage = 60.0f;
      MaxSpeed = 32.0f;
      MinSpeed = 0.0f;
      MaxSpeedChangeRate = 5.0f;
      MaxTurnRate = 2f;

      Score_perStrength = 15;
      Score_DestroyBonus = 10000;

      CullDistance = 30000;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"nebulonb\nebulonb.x");
      SourceFarMeshPath = Path.Combine(Globals.ModelPath, @"nebulonb\nebulonb_far.x");

      SoundSources = new SoundSourceInfo[] { new SoundSourceInfo("engine_big", 1500.0f, new TV_3DVECTOR(0, 100, -300), true) };
      AddOns = new AddOnInfo[]
      {
        new AddOnInfo("Nebulon B Turbolaser Tower", new TV_3DVECTOR(0, 40, 220), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnInfo("Nebulon B Turbolaser Tower", new TV_3DVECTOR(56, 90, -520), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnInfo("Nebulon B Turbolaser Tower", new TV_3DVECTOR(0, -180, -550), new TV_3DVECTOR(-90, 0, 0), true)
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

      ainfo.CameraSystemInfo.CamLocations = new TV_3DVECTOR[] { new TV_3DVECTOR(66, 78, -480) };
      ainfo.CameraSystemInfo.CamTargets = new TV_3DVECTOR[] { new TV_3DVECTOR(0, 75, 2000) };
    }
  }
}

