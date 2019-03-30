using MTV3D65;
using SWEndor.Weapons;
using SWEndor.Weapons.Types;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.Actors.Types
{
  public class BWingATI : FighterGroup
  {
    private static BWingATI _instance;
    public static BWingATI Instance()
    {
      if (_instance == null) { _instance = new BWingATI(); }
      return _instance;
    }

    private BWingATI() : base("B-Wing")
    {
      // Combat
      IsCombatObject = true;
      IsSelectable = true;
      IsDamage = false;
      CollisionEnabled = true;

      MaxStrength = 45;
      ImpactDamage = 16;
      MaxSpeed = 400;
      MinSpeed = 200;
      MaxSpeedChangeRate = 200;
      MaxTurnRate = 40;

      Score_perStrength = 400;
      Score_DestroyBonus = 2500;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"bwing\bwing.x");
      SourceFarMeshPath = Path.Combine(Globals.ModelPath, @"bwing\bwing_far.x");

      Debris = new DebrisSpawnerInfo[] {
        new DebrisSpawnerInfo(ActorTypeFactory.Instance().GetActorType("BWing_WingATI"), new TV_3DVECTOR(-30, -30, 0), -1000, 1000, 0, 3000, -2500, 2500, 0.5f),
        new DebrisSpawnerInfo(ActorTypeFactory.Instance().GetActorType("BWing_WingATI"), new TV_3DVECTOR(30, -30, 0), -1000, 1000, -3000, 0, -2500, 2500, 0.5f),
        new DebrisSpawnerInfo(ActorTypeFactory.Instance().GetActorType("BWing_Top_WingATI"), new TV_3DVECTOR(0, 0, 0), -1000, 1000, -1000, 1000, -2500, 2500, 0.5f),
        new DebrisSpawnerInfo(ActorTypeFactory.Instance().GetActorType("BWing_Bottom_WingATI"), new TV_3DVECTOR(0, -70, 0), -1000, 1000, -1000, 1000, -2500, 2500, 0.5f)
        };
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      // Camera System
      ainfo.CamLocations.Add(new TV_3DVECTOR(0, 0, 14));
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

      ainfo.RegenerationInfo.SelfRegenRate = 0.08f;

      ainfo.Weapons = new Dictionary<string, WeaponInfo>{ {"torp", new BWingTorpWeapon() }
                                                        , {"ion", WeaponFactory.Get("B-Wing Ion")} //new BWingIonWeapon() }
                                                        , {"laser", new BWingLaserWeapon() }
                                                        };
      ainfo.PrimaryWeapons = new string[] { "1:laser", "2:laser", "4:laser" };
      ainfo.SecondaryWeapons = new string[] { "4:laser", "1:ion", "1:torp" };
      ainfo.AIWeapons = new string[] { "1:ion", "1:torp", "1:laser" };
    }
  }
}

