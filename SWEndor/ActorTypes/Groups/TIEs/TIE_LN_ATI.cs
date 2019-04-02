using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes.Components;
using SWEndor.Weapons;
using SWEndor.Weapons.Types;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.ActorTypes
{
  public class TIE_LN_ATI : TIEGroup
  {
    private static TIE_LN_ATI _instance;
    public static TIE_LN_ATI Instance()
    {
      if (_instance == null) { _instance = new TIE_LN_ATI(); }
      return _instance;
    }

    private TIE_LN_ATI() : base("TIE")
    {
      MaxStrength = 4;
      ImpactDamage = 16;
      MaxSpeed = 350;
      MinSpeed = 175;
      MaxSpeedChangeRate = 200;
      MaxTurnRate = 45;

      Score_perStrength = 400;
      Score_DestroyBonus = 400;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"tie\tie.x");
      SourceFarMeshPath = Path.Combine(Globals.ModelPath, @"tie\tie_far.x");

    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      // Camera System
      ainfo.CamLocations.Add(new TV_3DVECTOR(0, 0, 12));
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

      ainfo.Weapons = new Dictionary<string, WeaponInfo>{ {"laser", new TIE_LN_LaserWeapon() }
                                                        , {"2xlsr", new TIE_LN_DblLaserWeapon() }
                                                        };
      ainfo.PrimaryWeapons = new string[] { "1:laser", "2:laser" };
      ainfo.SecondaryWeapons = new string[] { "none" };
      ainfo.AIWeapons = new string[] { "1:2xlsr" };

      Debris = new DebrisSpawnerInfo[] {
        new DebrisSpawnerInfo("TIE_WingATI", new TV_3DVECTOR(-30, 0, 0), -1000, 1000, 0, 3000, -2500, 2500, 0.5f),
        new DebrisSpawnerInfo("TIE_WingATI", new TV_3DVECTOR(30, 0, 0), -1000, 1000, -3000, 0, -2500, 2500, 0.5f),
        };
    }
  }
}

