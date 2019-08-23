using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Weapons;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class ACTurboLaserATI : Groups.Turbolasers
  {
    internal ACTurboLaserATI(Factory owner) : base(owner, "Acclamator Turbolaser Tower")
    {
      MaxStrength = 85;
      ImpactDamage = 16;

      Score_perStrength = 250;
      Score_DestroyBonus = 1250;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"turbotowers\acclamator_turbolaser.x");
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.DyingMoveComponent = DyingKill.Instance;

      ainfo.WeaponSystemInfo.Weapons = new Dictionary<string, WeaponInfo>{ {"laser", WeaponFactory.Get("ACCL_LASR") } };
      ainfo.WeaponSystemInfo.PrimaryWeapons = new string[] { "1:laser" };
      ainfo.WeaponSystemInfo.SecondaryWeapons = new string[] { "none" };
      ainfo.WeaponSystemInfo.AIWeapons = new string[] { "1:laser" };
    }
  }
}

