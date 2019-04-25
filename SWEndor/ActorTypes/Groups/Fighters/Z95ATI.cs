using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes.Components;
using SWEndor.Weapons;
using SWEndor.Weapons.Types;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Z95ATI : Groups.RebelWing
  {
    internal Z95ATI(Factory owner) : base(owner, "Z-95")
    {
      // Combat
      IsCombatObject = true;
      IsSelectable = true;
      IsDamage = false;
      CollisionEnabled = true;

      MaxStrength = 15;
      ImpactDamage = 16;
      MaxSpeed = 400;
      MinSpeed = 175;
      MaxSpeedChangeRate = 200;
      MaxTurnRate = 42;

      Score_perStrength = 400;
      Score_DestroyBonus = 2000;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"xwing\z95.x");
      //SourceFarMeshPath = Path.Combine(Globals.ModelPath, @"xwing\xwing_far.x");

      Debris = new DebrisSpawnerInfo[] {
        new DebrisSpawnerInfo("XWing_RU_LD_WingATI", new TV_3DVECTOR(-30, -10, 0), 0, 2000, 1000, 1000, -2500, 2500, 0.5f),
        new DebrisSpawnerInfo("XWing_RD_LU_WingATI", new TV_3DVECTOR(30, -10, 0), -2000, 0, -1000, 1000, -2500, 2500, 0.5f),
        };
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.CameraSystemInfo.CamLocations = new TV_3DVECTOR[]
      {
        new TV_3DVECTOR(0, 3, 25),
        new TV_3DVECTOR(0, 125, -200),
        new TV_3DVECTOR(0, 40, 250)
      };

      ainfo.CameraSystemInfo.CamTargets = new TV_3DVECTOR[]
      {
        new TV_3DVECTOR(0, 5, 2000),
        new TV_3DVECTOR(0, 0, 2000),
        new TV_3DVECTOR(0, 0, -2000)
      };

      ainfo.ExplosionInfo.EnableDeathExplosion = true;
      ainfo.ExplosionInfo.DeathExplosionType = "ExplosionSm";
      ainfo.ExplosionInfo.ExplosionRate = 0.75f;
      ainfo.ExplosionInfo.ExplosionSize = 1;
      ainfo.ExplosionInfo.ExplosionType = "Explosion";

      ainfo.RegenerationInfo.SelfRegenRate = 0.08f;

      ainfo.WeaponSystemInfo.Weapons = new Dictionary<string, WeaponInfo>{ {"laser", new Z95LaserWeapon() }
                                                        };
      ainfo.WeaponSystemInfo.PrimaryWeapons = new string[] { "1:laser", "2:laser" };
      ainfo.WeaponSystemInfo.SecondaryWeapons = new string[] { "none" };
      ainfo.WeaponSystemInfo.AIWeapons = new string[] { "1:laser" };
    }
  }
}

