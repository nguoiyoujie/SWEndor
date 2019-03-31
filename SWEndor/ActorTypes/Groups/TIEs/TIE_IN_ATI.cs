using MTV3D65;
using SWEndor.Weapons;
using SWEndor.Weapons.Types;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.Actors.Types
{
  public class TIE_IN_ATI : TIEGroup
  {
    private static TIE_IN_ATI _instance;
    public static TIE_IN_ATI Instance()
    {
      if (_instance == null) { _instance = new TIE_IN_ATI(); }
      return _instance;
    }

    private TIE_IN_ATI() : base("TIE Interceptor")
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

      // Camera System
      ainfo.CamLocations.Add(new TV_3DVECTOR(0, 0, 20));
      ainfo.CamTargets.Add(new TV_3DVECTOR(0, 0, 2000));
      ainfo.CamLocations.Add(new TV_3DVECTOR(0, 125, -200));
      ainfo.CamTargets.Add(new TV_3DVECTOR(0, 0, 2000));
      ainfo.CamLocations.Add(new TV_3DVECTOR(0, 40, 250));
      ainfo.CamTargets.Add(new TV_3DVECTOR(0, 0, -2000));

      ainfo.ExplosionInfo.EnableDeathExplosion = true;
      ainfo.ExplosionInfo.DeathExplosionType = "ExplosionSm";
      ainfo.ExplosionInfo.ExplosionRate = 0.75f;
      ainfo.ExplosionInfo.ExplosionSize = 1;
      ainfo.ExplosionInfo.ExplosionType = "Explosion";

      ainfo.Weapons = new Dictionary<string, WeaponInfo>{ {"laser", new TIE_IN_LaserWeapon() }
                                                        , {"2xlsr", new TIE_IN_DblLaserWeapon() }
                                                        };
      ainfo.PrimaryWeapons = new string[] { "1:laser", "2:laser" };
      ainfo.SecondaryWeapons = new string[] { "none" };
      ainfo.AIWeapons = new string[] { "1:2xlsr" };

      Debris = new DebrisSpawnerInfo[] {
        new DebrisSpawnerInfo("TIE_InterceptorWingATI", new TV_3DVECTOR(-30, 0, 0), -1000, 1000, 0, 3000, -2500, 2500, 0.5f),
        new DebrisSpawnerInfo("TIE_InterceptorWingATI", new TV_3DVECTOR(30, 0, 0), -1000, 1000, -3000, 0, -2500, 2500, 0.5f),
        };
    }
  }
}

