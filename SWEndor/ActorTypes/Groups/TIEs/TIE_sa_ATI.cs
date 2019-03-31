using MTV3D65;
using SWEndor.Weapons;
using SWEndor.Weapons.Types;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.Actors.Types
{
  public class TIE_sa_ATI : TIEGroup
  {
    private static TIE_sa_ATI _instance;
    public static TIE_sa_ATI Instance()
    {
      if (_instance == null) { _instance = new TIE_sa_ATI(); }
      return _instance;
    }

    private TIE_sa_ATI() : base("TIE Bomber")
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

      ainfo.Weapons = new Dictionary<string, WeaponInfo>{ {"torp", new TIE_sa_TorpWeapon() }
                                                        , {"ion", new TIE_sa_IonWeapon() }
                                                        , {"laser", new TIE_sa_LaserWeapon() }
                                                        };
      ainfo.PrimaryWeapons = new string[] { "1:laser", "2:laser" };
      ainfo.SecondaryWeapons = new string[] { "none", "1:ion", "1:torp" };
      ainfo.AIWeapons = new string[] { "1:torp", "1:ion", "1:laser" };
    }
  }
}

