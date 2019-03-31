using MTV3D65;
using SWEndor.Weapons;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.Actors.Types
{
  public class AWingATI : RebelWingGroup
  {
    private static AWingATI _instance;
    public static AWingATI Instance()
    {
      if (_instance == null) { _instance = new AWingATI(); }
      return _instance;
    }

    private AWingATI() : base("A-Wing")
    {
      // Combat
      IsCombatObject = true;
      IsSelectable = true;
      IsDamage = false;
      CollisionEnabled = true;

      MaxStrength = 18;
      ImpactDamage = 16;
      MaxSpeed = 500;
      MinSpeed = 250;
      MaxSpeedChangeRate = 300;
      MaxTurnRate = 55;

      Score_perStrength = 500;
      Score_DestroyBonus = 2500;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"awing\awing.x");
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      // Camera System
      ainfo.CamLocations.Add(new TV_3DVECTOR(0, 2, 20));
      ainfo.CamTargets.Add(new TV_3DVECTOR(0, 5, 2000));
      ainfo.CamLocations.Add(new TV_3DVECTOR(0, 125, -200));
      ainfo.CamTargets.Add(new TV_3DVECTOR(0, 0, 2000));
      ainfo.CamLocations.Add(new TV_3DVECTOR(0, 40, 250));
      ainfo.CamTargets.Add(new TV_3DVECTOR(0, 0, -2000));

      ainfo.ExplosionInfo.EnableDeathExplosion = true;
      ainfo.ExplosionInfo.ExplosionRate = 0.75f;
      ainfo.ExplosionInfo.ExplosionSize = 1;

      ainfo.RegenerationInfo.SelfRegenRate= 0.08f;

      ainfo.Weapons = new Dictionary<string, WeaponInfo>{ {"torp", WeaponFactory.Get("A-Wing Torpedo")} //new AWingTorpWeapon() }
                                                        , {"laser", WeaponFactory.Get("A-Wing Laser")} // new AWingLaserWeapon() }
                                                        };
      ainfo.PrimaryWeapons = new string[] { "1:laser", "2:laser", "4:laser" };
      ainfo.SecondaryWeapons = new string[] { "none", "1:torp" };
      ainfo.AIWeapons = new string[] { "1:torp", "1:laser" };
    }
  }
}

