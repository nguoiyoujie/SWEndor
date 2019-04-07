using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes.Components;
using SWEndor.Weapons;
using SWEndor.Weapons.Types;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.ActorTypes
{
  public class LandoFalconATI : RebelWingGroup
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

      SoundSources = new SoundSourceInfo[] { new SoundSourceInfo("falcon_engine", 200f, new TV_3DVECTOR(0, 0, -30), true) };
      AddOns = new AddOnInfo[] { new AddOnInfo("Invisible Rebel Turbo Laser", new TV_3DVECTOR(0, 7, 20), new TV_3DVECTOR(90, 0, 0), true) };
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

      ainfo.ExplosionInfo.EnableDeathExplosion = true;
      ainfo.ExplosionInfo.DeathExplosionType = "ExplosionSm";
      ainfo.ExplosionInfo.ExplosionRate = 1;
      ainfo.ExplosionInfo.ExplosionSize = 1;
      ainfo.ExplosionInfo.ExplosionType = "Explosion";

      ainfo.RegenerationInfo.SelfRegenRate = 0.1f;
      
      ainfo.Weapons = new Dictionary<string, WeaponInfo>{ {"laser", new FalconLaserWeapon() }
                                                        };
      ainfo.PrimaryWeapons = new string[] { "1:laser" };
      ainfo.SecondaryWeapons = new string[] { "none" };
      ainfo.AIWeapons = new string[] { "1:laser" };

      ainfo.CombatInfo.HitWhileDyingLeadsToDeath = false;
    }
  }
}

