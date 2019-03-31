using SWEndor.Weapons;
using SWEndor.Weapons.Types;
using System.Collections.Generic;

namespace SWEndor.Actors.Types
{
  public class InvisibleRebelTurboLaserATI : AddOnGroup
  {
    private static InvisibleRebelTurboLaserATI _instance;
    public static InvisibleRebelTurboLaserATI Instance()
    {
      if (_instance == null) { _instance = new InvisibleRebelTurboLaserATI(); }
      return _instance;
    }

    private InvisibleRebelTurboLaserATI() : base("Invisible Rebel Turbo Laser")
    {
      // Combat
      IsCombatObject = false;
      IsSelectable = false;
      IsDamage = false;
      CollisionEnabled = true;
      RadarSize = 0;

      TargetType = TargetType.NULL;
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.Weapons = new Dictionary<string, WeaponInfo>{ {"laser", new FalconGunPortLaserWeapon() }
                                                        };
      ainfo.PrimaryWeapons = new string[] { "1:laser" };
      ainfo.SecondaryWeapons = new string[] { "none" };
      ainfo.AIWeapons = new string[] { "1:laser" };
    }
  }
}

