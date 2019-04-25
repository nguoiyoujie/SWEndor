using MTV3D65;
using SWEndor.Actors;
using SWEndor.Weapons;
using SWEndor.Weapons.Types;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class MissileBoatATI : Groups.TIE
  {
    internal MissileBoatATI(Factory owner) : base(owner, "Missile Boat")
    {
      MaxStrength = 12;
      ImpactDamage = 16;
      MaxSpeed = 475;
      MinSpeed = 175;
      MaxSpeedChangeRate = 250;
      MaxTurnRate = 48;

      ZTilt = 0.75f;
      ZNormFrac = 0.006f;

      Score_perStrength = 800;
      Score_DestroyBonus = 2000;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"tie_vader\tie_vader.x");
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

      ainfo.ExplosionInfo.EnableDeathExplosion = true;
      ainfo.ExplosionInfo.DeathExplosionType = "ExplosionSm";
      ainfo.ExplosionInfo.ExplosionRate = 0.75f;
      ainfo.ExplosionInfo.ExplosionSize = 1;
      ainfo.ExplosionInfo.ExplosionType = "Explosion";

      ainfo.RegenerationInfo.SelfRegenRate = 0.075f;

      ainfo.WeaponSystemInfo.Weapons = new Dictionary<string, WeaponInfo>{ {"torp", new MissileBoat_TorpWeapon() }
                                                        , {"misl", new MissileBoat_MissileWeapon() }
                                                        , {"laser", new MissileBoat_LaserWeapon() }
                                                        };
      ainfo.WeaponSystemInfo.PrimaryWeapons = new string[] { "1:laser", "2:laser" };
      ainfo.WeaponSystemInfo.SecondaryWeapons = new string[] { "none", "1:misl", "2:misl", "1:torp", "2:torp" };
      ainfo.WeaponSystemInfo.AIWeapons = new string[] { "1:torp", "1:misl", "1:laser" };
    }
  }
}

