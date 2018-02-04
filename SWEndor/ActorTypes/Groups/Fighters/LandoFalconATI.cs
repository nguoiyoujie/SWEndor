using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class LandoFalconATI : FighterGroup
  {
    private static LandoFalconATI _instance;
    public static LandoFalconATI Instance()
    {
      if (_instance == null) { _instance = new LandoFalconATI(); }
      return _instance;
    }

    private LandoFalconATI() : base("Millennium Falcon (Lando)")
    {
      // Combat
      IsCombatObject = true;
      IsSelectable = true;
      IsDamage = false;
      CollisionEnabled = true;

      MaxStrength = 200;
      ImpactDamage = 10;
      MaxSpeed = 500;
      MinSpeed = 250;
      MaxSpeedChangeRate = 250;
      MaxTurnRate = 55;

      AggressiveTracker = true;
      Score_perStrength = 750;
      Score_DestroyBonus = 10000;

      AlwaysShowInRadar = true;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"falcon\falcon.x");
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      // Weapon selection
      ainfo.SetStateS("PrimaryWeaponModes", "1laser");
      ainfo.SetStateS("SecondaryWeaponModes", "none");

      // Camera System
      ainfo.CamLocations.Add(new TV_3DVECTOR(0, 0, 50));
      ainfo.CamTargets.Add(new TV_3DVECTOR(0, 5, 2000));
      ainfo.CamLocations.Add(new TV_3DVECTOR(0, 125, -200));
      ainfo.CamTargets.Add(new TV_3DVECTOR(0, 0, 2000));
      ainfo.CamLocations.Add(new TV_3DVECTOR(0, 40, 250));
      ainfo.CamTargets.Add(new TV_3DVECTOR(0, 0, -2000));

      ainfo.EnableDeathExplosion = true;
      ainfo.DeathExplosionType = "ExplosionSm";
      ainfo.ExplosionRate = 1;
      ainfo.ExplosionSize = 1;
      ainfo.ExplosionType = "Explosion";

      ainfo.SelfRegenRate = 0.5f;
      
      ainfo.SetStateS("AddOn_0", "Invisible Rebel Turbo Laser, 0, 7, 20, 0, 0, 0, true");
      //ainfo.SetCustomStateS("AddOn_1", "Invisible Rebel Turbo Laser, 0, -2, 10, 0, 0, 0, true");

      ainfo.Weapons = new Dictionary<string, WeaponInfo>{ {"laser", new FalconLaserWeapon() }
                                                        };
      ainfo.PrimaryWeapons = new List<string> { "1:laser" };
      ainfo.SecondaryWeapons = new List<string> { "none" };
      ainfo.AIWeapons = new List<string> { "1:laser" };

      ainfo.SetStateB("No2ndKill", true);
    }
  }
}

