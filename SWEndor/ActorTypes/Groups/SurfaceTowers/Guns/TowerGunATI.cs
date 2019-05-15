using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Actors.Data;
using SWEndor.Weapons;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class TowerGunATI : Groups.SurfaceGun
  {
    internal TowerGunATI(Factory owner) : base(owner, "Turbolaser Turret")
    {
      ExplodeData = new ExplodeData(deathTrigger: DeathExplosionTrigger.ALWAYS, deathExplosionSize: 3);

      MaxStrength = 16;
      ImpactDamage = 60;
      MaxTurnRate = 36;
      ZTilt = 0;
      XLimit = 35;

      Score_perStrength = 25;
      Score_DestroyBonus = 1500;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"towers\tower_turbolaser.x");
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.WeaponSystemInfo.Weapons = new Dictionary<string, WeaponInfo> { { "laser", WeaponFactory.Get("TOWR_DLSR") }
                                                        };
      ainfo.WeaponSystemInfo.PrimaryWeapons = new string[] { "1:laser" };
      ainfo.WeaponSystemInfo.SecondaryWeapons = new string[] { "none" };
      ainfo.WeaponSystemInfo.AIWeapons = new string[] { "1:laser" };
    }
  }
}

