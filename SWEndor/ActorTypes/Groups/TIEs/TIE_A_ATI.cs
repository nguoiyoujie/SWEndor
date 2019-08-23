using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Data;
using SWEndor.ActorTypes.Components;
using SWEndor.Weapons;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class TIE_A_ATI : Groups.TIE
  {
    internal TIE_A_ATI(Factory owner) : base(owner, "TIE Avenger")
    {
      MaxStrength = 15;
      ImpactDamage = 16;
      MaxSpeed = 650;
      MinSpeed = 200;
      MaxSpeedChangeRate = 350;
      MaxTurnRate = 60;

      ZTilt = 1.75f;
      ZNormFrac = 0.005f;

      Score_perStrength = 800;
      Score_DestroyBonus = 2000;

      RegenData = new RegenData { SelfRegenRate = 0.06f };

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"tie_vader\tie_vader.x");

      Cameras = new ActorCameraInfo[]
      {
        new ActorCameraInfo(new TV_3DVECTOR(0, 0, 20), new TV_3DVECTOR(0, 0, 2000)),
        new ActorCameraInfo(new TV_3DVECTOR(0, 25, -100), new TV_3DVECTOR(0, 0, 2000)),
        new ActorCameraInfo(new TV_3DVECTOR(0, 0, -40), new TV_3DVECTOR(0, 0, -2000))
      };
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.WeaponSystemInfo.Weapons = new Dictionary<string, WeaponInfo>{ {"misl", WeaponFactory.Get("TIEA_MISL") }
                                                        , {"laser", WeaponFactory.Get("TIEA_LASR") }
                                                        };
      ainfo.WeaponSystemInfo.PrimaryWeapons = new string[] { "1:laser", "2:laser" };
      ainfo.WeaponSystemInfo.SecondaryWeapons = new string[] { "none", "1:misl" };
      ainfo.WeaponSystemInfo.AIWeapons = new string[] { "1:misl", "1:laser" };
    }
  }
}

