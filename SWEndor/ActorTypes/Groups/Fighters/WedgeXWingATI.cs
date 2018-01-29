using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class WedgeXWingATI : FighterGroup
  {
    private static WedgeXWingATI _instance;
    public static WedgeXWingATI Instance()
    {
      if (_instance == null) { _instance = new WedgeXWingATI(); }
      return _instance;
    }

    private WedgeXWingATI() : base("X-Wing (Wedge)")
    {
      // Combat
      IsCombatObject = true;
      IsSelectable = true;
      IsDamage = false;
      CollisionEnabled = true;

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

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"xwing\xwing.x");
      SourceFarMeshPath = Path.Combine(Globals.ModelPath, @"xwing\xwing_far.x");
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      // Camera System
      ainfo.CamLocations.Add(new TV_3DVECTOR(0, 3, 25));
      ainfo.CamTargets.Add(new TV_3DVECTOR(0, 5, 2000));
      ainfo.CamLocations.Add(new TV_3DVECTOR(0, 125, -200));
      ainfo.CamTargets.Add(new TV_3DVECTOR(0, 0, 2000));
      ainfo.CamLocations.Add(new TV_3DVECTOR(0, 40, 250));
      ainfo.CamTargets.Add(new TV_3DVECTOR(0, 0, -2000));

      ainfo.EnableDeathExplosion = true;
      ainfo.DeathExplosionType = "ExplosionSm";
      ainfo.ExplosionRate = 0.75f;
      ainfo.ExplosionSize = 1;
      ainfo.ExplosionType = "Explosion";

      ainfo.SelfRegenRate = 0.6f;

      ainfo.Weapons = new Dictionary<string, WeaponInfo>{ {"torp", new XWingTorpWeapon() }
                                                        , {"laser", new XWingLaserWeapon() }
                                                        };
      ainfo.PrimaryWeapons = new List<string> { "1:laser", "2:laser", "4:laser" };
      ainfo.SecondaryWeapons = new List<string> { "4:laser", "1:torp" };
      ainfo.AIWeapons = new List<string> { "1:torp", "1:laser" };

      ainfo.SetStateB("No2ndKill", true);
    }
  }
}

