using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class NebulonBATI : Groups.Warship
  {
    internal NebulonBATI(Factory owner) : base(owner, "Nebulon-B Frigate")
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

      ainfo.DyingMovement = new DyingSinkInfo(0.02f, 5f, 0.8f);

      ainfo.RegenerationInfo.SelfRegenRate = 0.1f;

      ainfo.CameraSystemInfo.CamLocations = new TV_3DVECTOR[] { new TV_3DVECTOR(66, 78, -480) };
      ainfo.CameraSystemInfo.CamTargets = new TV_3DVECTOR[] { new TV_3DVECTOR(0, 75, 2000) };
    }
  }
}

