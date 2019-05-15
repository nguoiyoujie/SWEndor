using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.ActorTypes.Components;
using SWEndor.Weapons;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class TIE_sa_ATI : Groups.TIE
  {
    internal TIE_sa_ATI(Factory owner) : base(owner, "TIE Bomber")
    {
      MaxStrength = 6;
      ImpactDamage = 16;
      MaxSpeed = 300;
      MinSpeed = 150;
      MaxSpeedChangeRate = 200;
      MaxTurnRate = 40;

      Score_perStrength = 350;
      Score_DestroyBonus = 500;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"tie_vader\tie_bomber.x");

      Cameras = new ActorCameraInfo[]
      {
        new ActorCameraInfo(new TV_3DVECTOR(0, 0, 20), new TV_3DVECTOR(0, 0, 2000)),
        new ActorCameraInfo(new TV_3DVECTOR(0, 0, -40), new TV_3DVECTOR(0, 0, -2000))
      };
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.WeaponSystemInfo.Weapons = new Dictionary<string, WeaponInfo>{ {"torp", WeaponFactory.Get("TIEB_TORP") }
                                                        , {"ion", WeaponFactory.Get("TIEB_ION") }
                                                        , {"laser", WeaponFactory.Get("TIEB_LASR") }
                                                        };
      ainfo.WeaponSystemInfo.PrimaryWeapons = new string[] { "1:laser", "2:laser" };
      ainfo.WeaponSystemInfo.SecondaryWeapons = new string[] { "none", "1:ion", "1:torp" };
      ainfo.WeaponSystemInfo.AIWeapons = new string[] { "1:torp", "1:ion", "1:laser" };
    }
  }
}

