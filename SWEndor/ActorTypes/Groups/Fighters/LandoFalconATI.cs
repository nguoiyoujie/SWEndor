using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Actors.Data;
using SWEndor.ActorTypes.Components;
using SWEndor.Weapons;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class LandoFalconATI : Groups.RebelWing
  {
    internal LandoFalconATI(Factory owner) : base(owner, "Millennium Falcon (Lando)")
    {
      // Combat
      CombatData = CombatData.DefaultShip;
      ExplodeData = new ExplodeData(deathTrigger: DeathExplosionTrigger.ALWAYS);

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

      RegenData = new RegenData { SelfRegenRate = 0.1f };

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"falcon\falcon.x");

      SoundSources = new SoundSourceInfo[] { new SoundSourceInfo("falcon_engine", 200f, new TV_3DVECTOR(0, 0, -30), true) };
      AddOns = new AddOnInfo[] { new AddOnInfo("Invisible Rebel Turbo Laser", new TV_3DVECTOR(0, 7, 20), new TV_3DVECTOR(90, 0, 0), true) };
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.CameraSystemInfo.CamLocations = new TV_3DVECTOR[]
      {
        new TV_3DVECTOR(0, 0, 50),
        new TV_3DVECTOR(0, 125, -200),
        new TV_3DVECTOR(0, 40, 250)
      };

      ainfo.CameraSystemInfo.CamTargets = new TV_3DVECTOR[]
      {
        new TV_3DVECTOR(0, 5, 2000),
        new TV_3DVECTOR(0, 0, 2000),
        new TV_3DVECTOR(0, 0, -2000)
      };

      ainfo.WeaponSystemInfo.Weapons = new Dictionary<string, WeaponInfo>{ {"laser", WeaponFactory.Get("FALC_LASR") }
                                                        };
      ainfo.WeaponSystemInfo.PrimaryWeapons = new string[] { "1:laser" };
      ainfo.WeaponSystemInfo.SecondaryWeapons = new string[] { "none" };
      ainfo.WeaponSystemInfo.AIWeapons = new string[] { "1:laser" };
    }
  }
}

