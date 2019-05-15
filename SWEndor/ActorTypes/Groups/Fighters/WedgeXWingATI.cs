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
  public class WedgeXWingATI : Groups.RebelWing
  {
    internal WedgeXWingATI(Factory owner) : base(owner, "X-Wing (Wedge)")
    {
      // Combat
      CombatData = CombatData.DefaultShip;

      MaxStrength = 200;
      ImpactDamage = 16;
      MaxSpeed = 500;
      MinSpeed = 250;
      MaxSpeedChangeRate = 500;
      MaxTurnRate = 50;

      AggressiveTracker = true;
      Score_perStrength = 750;
      Score_DestroyBonus = 10000;

      AlwaysShowInRadar = true;

      RegenData = new RegenData { SelfRegenRate = 0.1f };

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"xwing\xwing.x");
      SourceFarMeshPath = Path.Combine(Globals.ModelPath, @"xwing\xwing_far.x");

      Cameras = new ActorCameraInfo[] {
        new ActorCameraInfo(new TV_3DVECTOR(0, 3, 25), new TV_3DVECTOR(0, 3, 2000)),
        new ActorCameraInfo(new TV_3DVECTOR(0, 0, -40), new TV_3DVECTOR(0, 0, -2000))
        };
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.WeaponSystemInfo.Weapons = new Dictionary<string, WeaponInfo>{ {"torp", WeaponFactory.Get("X_WG_TORP") }
                                                        , {"laser", WeaponFactory.Get("X_WG_LASR") }
                                                        };
      ainfo.WeaponSystemInfo.PrimaryWeapons = new string[] { "1:laser", "2:laser", "4:laser" };
      ainfo.WeaponSystemInfo.SecondaryWeapons = new string[] { "none", "1:torp" };
      ainfo.WeaponSystemInfo.AIWeapons = new string[] { "1:torp", "1:laser" };

    }
  }
}

