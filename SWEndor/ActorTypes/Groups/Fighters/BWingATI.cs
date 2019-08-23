using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Data;
using SWEndor.ActorTypes.Components;
using SWEndor.Weapons;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class BWingATI : Groups.RebelWing
  {
    internal BWingATI(Factory owner) : base(owner, "B-Wing")
    {
      MaxStrength = 45;
      ImpactDamage = 16;
      MaxSpeed = 400;
      MinSpeed = 200;
      MaxSpeedChangeRate = 200;
      MaxTurnRate = 40;

      Score_perStrength = 400;
      Score_DestroyBonus = 2500;

      RegenData = new RegenData { SelfRegenRate = 0.08f };

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"bwing\bwing.x");
      SourceFarMeshPath = Path.Combine(Globals.ModelPath, @"bwing\bwing_far.x");

      Cameras = new ActorCameraInfo[] {
        new ActorCameraInfo(new TV_3DVECTOR(0, 0, 14), new TV_3DVECTOR(0, 0, 2000)),
        new ActorCameraInfo(new TV_3DVECTOR(0, 25, -100), new TV_3DVECTOR(0, 0, 2000)),
        new ActorCameraInfo(new TV_3DVECTOR(0, 0, -40), new TV_3DVECTOR(0, 0, -2000))
     };

      Debris = new DebrisSpawnerInfo[] {
        new DebrisSpawnerInfo("BWing_WingATI", new TV_3DVECTOR(-30, -30, 0), -1000, 1000, 0, 3000, -2500, 2500, 0.5f),
        new DebrisSpawnerInfo("BWing_WingATI", new TV_3DVECTOR(30, -30, 0), -1000, 1000, -3000, 0, -2500, 2500, 0.5f),
        new DebrisSpawnerInfo("BWing_Top_WingATI", new TV_3DVECTOR(0, 0, 0), -1000, 1000, -1000, 1000, -2500, 2500, 0.5f),
        new DebrisSpawnerInfo("BWing_Bottom_WingATI", new TV_3DVECTOR(0, -70, 0), -1000, 1000, -1000, 1000, -2500, 2500, 0.5f)
        };
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.WeaponSystemInfo.Weapons = new Dictionary<string, WeaponInfo>{ {"torp", WeaponFactory.Get("B_WG_TORP") }
                                                                         , {"ion", WeaponFactory.Get("B_WG_ION")}
                                                                         , {"laser", WeaponFactory.Get("B_WG_LASR") }
                                                                         };
      ainfo.WeaponSystemInfo.PrimaryWeapons = new string[] { "1:laser", "2:laser", "4:laser" };
      ainfo.WeaponSystemInfo.SecondaryWeapons = new string[] { "none", "1:ion", "1:torp" };
      ainfo.WeaponSystemInfo.AIWeapons = new string[] { "1:ion", "1:torp", "1:laser" };
    }
  }
}

