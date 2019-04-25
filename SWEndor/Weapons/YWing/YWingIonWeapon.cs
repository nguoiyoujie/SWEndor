using MTV3D65;
using SWEndor.ActorTypes;
using SWEndor.ActorTypes.Instances;

namespace SWEndor.Weapons.Types
{
  public class YWingIonWeapon : WeaponInfo
  {
    public YWingIonWeapon() : base("Y-Wing Ion", "Ion Laser")
    {
      WeaponCooldownRate = 0.15f;
      WeaponCooldownRateRandom = 0;

      Ammo = 10;
      MaxAmmo = 10;
      AmmoReloadRate = 5;
      AmmoReloadRateRandom = 0;
      AmmoReloadAmount = 1;

      FirePositions = new TV_3DVECTOR[] { new TV_3DVECTOR(0, -10, 70)
                                        };

      // Auto Aim Bot
      EnablePlayerAutoAim = false;
      EnableAIAutoAim = true;

      // Player Config
      RequirePlayerTargetLock = false;

      // AI Config
      AIAttackTargets = TargetType.SHIP | TargetType.ADDON;
      AIAttackNull = false;

      Range = 3000;
    }
  }
}
