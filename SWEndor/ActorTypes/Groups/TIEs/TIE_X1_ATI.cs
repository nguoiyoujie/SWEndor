using MTV3D65;
using SWEndor.Actors;
using SWEndor.Weapons;
using SWEndor.Weapons.Types;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.ActorTypes
{
  public class TIE_X1_ATI : TIEGroup
  {
    private static TIE_X1_ATI _instance;
    public static TIE_X1_ATI Instance()
    {
      if (_instance == null) { _instance = new TIE_X1_ATI(); }
      return _instance;
    }

    private TIE_X1_ATI() : base("TIE Advanced X1")
    {
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

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"tie_vader\tie_vader.x");
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      // Camera System
      ainfo.CamLocations.Add(new TV_3DVECTOR(0, 0, 20));
      ainfo.CamTargets.Add(new TV_3DVECTOR(0, 0, 2000));
      ainfo.CamLocations.Add(new TV_3DVECTOR(0, 125, -200));
      ainfo.CamTargets.Add(new TV_3DVECTOR(0, 0, 2000));
      ainfo.CamLocations.Add(new TV_3DVECTOR(0, 40, 250));
      ainfo.CamTargets.Add(new TV_3DVECTOR(0, 0, -2000));

      ainfo.ExplosionInfo.EnableDeathExplosion = true;
      ainfo.ExplosionInfo.DeathExplosionType = "ExplosionSm";
      ainfo.ExplosionInfo.DeathExplosionSize = 1;
      ainfo.ExplosionInfo.ExplosionRate = 0.5f;
      ainfo.ExplosionInfo.ExplosionSize = 1;
      ainfo.ExplosionInfo.ExplosionType = "Explosion";

      ainfo.RegenerationInfo.SelfRegenRate = 0.075f;

      ainfo.WeaponSystemInfo.Weapons = new Dictionary<string, WeaponInfo>{ {"torp", new TIE_X1_TorpWeapon() }
                                                        , {"laser", new TIE_X1_LaserWeapon() }
                                                        };
      ainfo.WeaponSystemInfo.PrimaryWeapons = new string[] { "1:laser", "2:laser", "3:laser" };
      ainfo.WeaponSystemInfo.SecondaryWeapons = new string[] { "none", "1:torp" };
      ainfo.WeaponSystemInfo.AIWeapons = new string[] { "1:torp", "2:laser" };

      ainfo.CombatInfo.HitWhileDyingLeadsToDeath = false;
    }
  }
}

