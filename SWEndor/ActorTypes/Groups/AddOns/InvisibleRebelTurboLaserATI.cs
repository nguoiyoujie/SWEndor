using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
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
      IsCombatObject = true;
      IsSelectable = false;
      IsDamage = false;
      CollisionEnabled = true;

      MaxStrength = 25.0f;
      ImpactDamage = 40.0f;
      MaxTurnRate = 50f;

      Score_perStrength = 250;
      Score_DestroyBonus = 1250;
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.Weapons = new Dictionary<string, WeaponInfo>{ {"laser", new FalconGunPortLaserWeapon() }
                                                        };
      ainfo.PrimaryWeapons = new List<string> { "1:laser" };
      ainfo.SecondaryWeapons = new List<string> { "none" };
      ainfo.AIWeapons = new List<string> { "1:laser" };
    }
  }
}

