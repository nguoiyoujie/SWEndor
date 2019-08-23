using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Data;
using SWEndor.ActorTypes.Components;
using SWEndor.Weapons;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class TIE_X1_ATI : Groups.TIE
  {
    internal TIE_X1_ATI(Factory owner) : base(owner, "TIE Advanced X1")
    {
      CombatData = CombatData.DefaultShip;
      Armor = ActorInfo.ArmorModel.Default;

      MaxStrength = 275;
      ImpactDamage = 100;
      MaxSpeed = 900;
      MinSpeed = 200;
      MaxSpeedChangeRate = 500;
      MaxTurnRate = 90;
      MaxSecondOrderTurnRateFrac = 0.8f;

      ZTilt = 3.25f;
      ZNormFrac = 0.005f;

      Attack_AngularDelta = 7.5f;
      Attack_HighAccuracyAngularDelta = 5;
      Move_CloseEnough = 1000;

      AggressiveTracker = true;

      Score_perStrength = 750;
      Score_DestroyBonus = 10000;

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

      ainfo.WeaponSystemInfo.Weapons = new Dictionary<string, WeaponInfo>{ {"torp", WeaponFactory.Get("TIEX_TORP") }
                                                        , {"laser", WeaponFactory.Get("TIEX_LASR") }
                                                        };
      ainfo.WeaponSystemInfo.PrimaryWeapons = new string[] { "1:laser", "2:laser", "3:laser" };
      ainfo.WeaponSystemInfo.SecondaryWeapons = new string[] { "none", "1:torp" };
      ainfo.WeaponSystemInfo.AIWeapons = new string[] { "1:torp", "2:laser" };
    }
  }
}

