using MTV3D65;
using SWEndor.ActorTypes;

namespace SWEndor.Weapons.Types
{
  public class TIE_sa_IonWeapon : WeaponInfo
  {
    public TIE_sa_IonWeapon() : base("TIE/sa Ion", "Ion Laser")
    {
      WeaponCooldownRate = 0.1f;
      WeaponCooldownRateRandom = 0;

      Ammo = 3;
      MaxAmmo = 3;
      AmmoReloadRate = 10;
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
