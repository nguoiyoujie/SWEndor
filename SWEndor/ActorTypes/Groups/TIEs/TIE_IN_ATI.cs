using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.ActorTypes.Components;
using SWEndor.Weapons;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class TIE_IN_ATI : Groups.TIE
  {
    internal TIE_IN_ATI(Factory owner) : base(owner, "TIE Interceptor")
    {
      MaxStrength = 8;
      ImpactDamage = 16;
      MaxSpeed = 500;
      MinSpeed = 250;
      MaxSpeedChangeRate = 200;
      MaxTurnRate = 55;

      Score_perStrength = 500;
      Score_DestroyBonus = 1000;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"tie\tie_interceptor.x");
      SourceFarMeshPath = Path.Combine(Globals.ModelPath, @"tie\tie_interceptor_far.x");
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.CameraSystemInfo.CamLocations = new TV_3DVECTOR[]
      {
        new TV_3DVECTOR(0, 0, 20),
        new TV_3DVECTOR(0, 125, -200),
        new TV_3DVECTOR(0, 40, 250)
      };

      ainfo.CameraSystemInfo.CamTargets = new TV_3DVECTOR[]
      {
        new TV_3DVECTOR(0, 0, 2000),
        new TV_3DVECTOR(0, 0, 2000),
        new TV_3DVECTOR(0, 0, -2000)
      };

      ainfo.WeaponSystemInfo.Weapons = new Dictionary<string, WeaponInfo>{ {"laser", WeaponFactory.Get("TIEI_LASR") }
                                                        , {"2xlsr", WeaponFactory.Get("TIEI_LASR_AI") }
                                                        };
      ainfo.WeaponSystemInfo.PrimaryWeapons = new string[] { "1:laser", "2:laser" };
      ainfo.WeaponSystemInfo.SecondaryWeapons = new string[] { "none" };
      ainfo.WeaponSystemInfo.AIWeapons = new string[] { "1:2xlsr" };

      Debris = new DebrisSpawnerInfo[] {
        new DebrisSpawnerInfo("TIE_InterceptorWingATI", new TV_3DVECTOR(-30, 0, 0), -1000, 1000, 0, 3000, -2500, 2500, 0.5f),
        new DebrisSpawnerInfo("TIE_InterceptorWingATI", new TV_3DVECTOR(30, 0, 0), -1000, 1000, -3000, 0, -2500, 2500, 0.5f),
        };
    }
  }
}

