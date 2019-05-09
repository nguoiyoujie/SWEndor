using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Actors.Data;
using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class NebulonB2ATI : Groups.Warship
  {
    internal NebulonB2ATI(Factory owner) : base(owner, "Nebulon-B2 Frigate")
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

      RegenData = new RegenData { SelfRegenRate = 0.25f };

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"nebulonb\nebulonb2.x");

      SoundSources = new SoundSourceInfo[] { new SoundSourceInfo("engine_big", 1500.0f, new TV_3DVECTOR(0, 100, -300), true) };
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

      ainfo.DyingMoveComponent = new DyingSink(0.02f, 5f, 0.8f);

      ainfo.CameraSystemInfo.CamLocations = new TV_3DVECTOR[] { new TV_3DVECTOR(0, 120, -300) };
      ainfo.CameraSystemInfo.CamTargets = new TV_3DVECTOR[] { new TV_3DVECTOR(0, 120, 2000) };
    }
  }
}

