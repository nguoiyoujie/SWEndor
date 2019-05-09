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
  public class YWingATI : Groups.RebelWing
  {
    internal YWingATI(Factory owner) : base(owner, "Y-Wing")
    {
      MaxStrength = 36;
      ImpactDamage = 16;
      MaxSpeed = 300;
      MinSpeed = 150;
      MaxSpeedChangeRate = 150;
      MaxTurnRate = 32;

      Score_perStrength = 400;
      Score_DestroyBonus = 2000;

      RegenData = new RegenData { SelfRegenRate = 0.08f };

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"ywing\ywing.x");
      SourceFarMeshPath = Path.Combine(Globals.ModelPath, @"ywing\ywing_far.x");
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.CameraSystemInfo.CamLocations = new TV_3DVECTOR[]
      {
        new TV_3DVECTOR(0, 4, 30),
        new TV_3DVECTOR(0, 125, -200),
        new TV_3DVECTOR(0, 40, 250)
      };

      ainfo.CameraSystemInfo.CamTargets = new TV_3DVECTOR[]
      {
        new TV_3DVECTOR(0, 5, 2000),
        new TV_3DVECTOR(0, 0, 2000),
        new TV_3DVECTOR(0, 0, -2000)
      };

      ainfo.WeaponSystemInfo.Weapons = new Dictionary<string, WeaponInfo>{ {"torp", WeaponFactory.Get("Y_WG_TORP") }
                                                        , {"ion", WeaponFactory.Get("Y_WG_ION")}
                                                        , {"laser", WeaponFactory.Get("Y_WG_LASR") }
                                                        };
      ainfo.WeaponSystemInfo.PrimaryWeapons = new string[] { "1:laser" };
      ainfo.WeaponSystemInfo.SecondaryWeapons = new string[] { "none", "1:ion", "1:torp" };
      ainfo.WeaponSystemInfo.AIWeapons = new string[] { "1:torp", "1:ion", "1:laser" };

      Debris = new DebrisSpawnerInfo[] {
        new DebrisSpawnerInfo("YWing_WingATI", new TV_3DVECTOR(-30, 0, 0), -1000, 1000, 0, 3000, -2500, 2500, 0.5f),
        new DebrisSpawnerInfo("YWing_WingATI", new TV_3DVECTOR(30, 0, 0), -1000, 1000, -3000, 0, -2500, 2500, 0.5f),
        };
    }
  }
}

