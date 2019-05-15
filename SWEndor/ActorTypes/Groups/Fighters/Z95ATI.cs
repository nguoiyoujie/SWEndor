using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Actors.Data;
using SWEndor.ActorTypes.Components;
using SWEndor.Weapons;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Z95ATI : Groups.RebelWing
  {
    internal Z95ATI(Factory owner) : base(owner, "Z-95")
    {
      MaxStrength = 15;
      ImpactDamage = 16;
      MaxSpeed = 400;
      MinSpeed = 175;
      MaxSpeedChangeRate = 200;
      MaxTurnRate = 42;

      Score_perStrength = 400;
      Score_DestroyBonus = 2000;

      RegenData = new RegenData { SelfRegenRate = 0.06f };

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"xwing\z95.x");
      //SourceFarMeshPath = Path.Combine(Globals.ModelPath, @"xwing\xwing_far.x");

      Cameras = new ActorCameraInfo[] {
        new ActorCameraInfo(new TV_3DVECTOR(0, 3, 25), new TV_3DVECTOR(0, 3, 2000)),
        new ActorCameraInfo(new TV_3DVECTOR(0, 0, -40), new TV_3DVECTOR(0, 0, -2000))
        };

      Debris = new DebrisSpawnerInfo[] {
        new DebrisSpawnerInfo("XWing_RU_LD_WingATI", new TV_3DVECTOR(-30, -10, 0), 0, 2000, 1000, 1000, -2500, 2500, 0.5f),
        new DebrisSpawnerInfo("XWing_RD_LU_WingATI", new TV_3DVECTOR(30, -10, 0), -2000, 0, -1000, 1000, -2500, 2500, 0.5f),
        };
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.WeaponSystemInfo.Weapons = new Dictionary<string, WeaponInfo>{ {"laser", WeaponFactory.Get("Z95__LASR") }
                                                        };
      ainfo.WeaponSystemInfo.PrimaryWeapons = new string[] { "1:laser", "2:laser" };
      ainfo.WeaponSystemInfo.SecondaryWeapons = new string[] { "none" };
      ainfo.WeaponSystemInfo.AIWeapons = new string[] { "1:laser" };
    }
  }
}

