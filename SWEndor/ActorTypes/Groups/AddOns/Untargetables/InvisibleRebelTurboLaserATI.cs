using SWEndor.Actors;
using SWEndor.Weapons;
using System.Collections.Generic;

namespace SWEndor.ActorTypes.Instances
{
  public class InvisibleRebelTurboLaserATI : Groups.AddOn
  {
    internal InvisibleRebelTurboLaserATI(Factory owner) : base(owner, "Invisible Rebel Turbo Laser")
    {
      RadarSize = 0;

      TargetType = TargetType.NULL;

      Mask &= ~(ComponentMask.CAN_BECOLLIDED | ComponentMask.CAN_BETARGETED);
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.WeaponSystemInfo.Weapons = new Dictionary<string, WeaponInfo>{ {"laser", WeaponFactory.Get("FALC_GLSR") }
                                                        };
      ainfo.WeaponSystemInfo.PrimaryWeapons = new string[] { "1:laser" };
      ainfo.WeaponSystemInfo.SecondaryWeapons = new string[] { "none" };
      ainfo.WeaponSystemInfo.AIWeapons = new string[] { "1:laser" };
    }
  }
}

