using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Weapons;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class SDAntiShipTurboLaserATI : Groups.Turbolasers
  {
    internal SDAntiShipTurboLaserATI(Factory owner) : base(owner, "Star Destroyer Heavy Turbolaser Tower")
    {
      MaxStrength = 12; //25
      ImpactDamage = 16;

      Score_perStrength = 250;
      Score_DestroyBonus = 1250;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"turbotowers\star_destroyer_anti-ship_turbolaser.x");
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.DyingMoveComponent = DyingKill.Instance;

      ainfo.WeaponSystemInfo.Weapons = new Dictionary<string, WeaponInfo>{ {"aship", WeaponFactory.Get("IMPL_3LSR") }
                                                        };
      ainfo.WeaponSystemInfo.PrimaryWeapons = new string[] { "1:aship" };
      ainfo.WeaponSystemInfo.SecondaryWeapons = new string[] { "none" };
      ainfo.WeaponSystemInfo.AIWeapons = new string[] { "1:aship" };
    }
  }
}

