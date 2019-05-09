using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Weapons;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class ExecutorTurboLaserATI : Groups.Turbolasers
  {
    internal ExecutorTurboLaserATI(Factory owner) : base(owner, "Executor Super Star Destroyer Turbolaser Tower")
    {
      MaxStrength = 105;
      ImpactDamage = 16;

      Score_perStrength = 300;
      Score_DestroyBonus = 2500;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"turbotowers\executor_turbolaser.x");
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.DyingMoveComponent = DyingKill.Instance;

      ainfo.WeaponSystemInfo.Weapons = new Dictionary<string, WeaponInfo>{ {"laser", WeaponFactory.Get("EXEC_LASR")}
                                                        };
      ainfo.WeaponSystemInfo.PrimaryWeapons = new string[] { "1:laser" };
      ainfo.WeaponSystemInfo.SecondaryWeapons = new string[] { "none" };
      ainfo.WeaponSystemInfo.AIWeapons = new string[] { "1:laser" };
    }
  }
}

