using MTV3D65;
using SWEndor.ActorTypes;
using SWEndor.ActorTypes.Instances;

namespace SWEndor.Weapons.Types
{
  public class XWingTorpWeapon : WeaponInfo
  {
    public XWingTorpWeapon() : base("X-Wing Torpedo", "Torpedo")
    {
      WeaponCooldownRate = 0.25f;
      WeaponCooldownRateRandom = 0;

      Ammo = 6;
      MaxAmmo = 6;
      AmmoReloadRate = 15;
      AmmoReloadRateRandom = 5;
      AmmoReloadAmount = 1;

      FirePositions = new TV_3DVECTOR[] { new TV_3DVECTOR(0, -10, 100)
                                        };

      // Auto Aim Bot
      EnablePlayerAutoAim = true;
      EnableAIAutoAim = true;

      // Player Config
      RequirePlayerTargetLock = true;

      // AI Config
      AIAttackTargets = TargetType.SHIP | TargetType.ADDON;
      AIAttackNull = false;

      // 
      FireSound = "torpedo";

      AngularRange = 5;
    }
  }
}
