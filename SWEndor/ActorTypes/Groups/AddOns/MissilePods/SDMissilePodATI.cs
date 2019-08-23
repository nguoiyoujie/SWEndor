using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Weapons;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class SDMissilePodATI : Groups.Turbolasers
  {
    internal SDMissilePodATI(Factory owner) : base(owner, "Star Destroyer Missile Pod")
    {
      MaxStrength = 24; //32
      ImpactDamage = 16;

      Score_perStrength = 250;
      Score_DestroyBonus = 1250;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"turbotowers\star_destroyer_missilepod.x");
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.DyingMoveComponent = DyingKill.Instance;

      ainfo.WeaponSystemInfo.Weapons = new Dictionary<string, WeaponInfo>{ {"missile", WeaponFactory.Get("IMPL_MISL") }
                                                        };
      ainfo.WeaponSystemInfo.PrimaryWeapons = new string[] { "1:missile" };
      ainfo.WeaponSystemInfo.SecondaryWeapons = new string[] { "none" };
      ainfo.WeaponSystemInfo.AIWeapons = new string[] { "1:missile" };
    }
  }
}

