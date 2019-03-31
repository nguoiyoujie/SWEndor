using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Types;

namespace SWEndor.Weapons.Types
{
  public class TIE_X1_TorpWeapon : WeaponInfo
  {
    public TIE_X1_TorpWeapon() : base("TIE/X1 Torpedo")
    {
      WeaponProjectile = TorpedoATI.Instance();
      WeaponCooldownRate = 0.15f;
      WeaponCooldownRateRandom = 0;

      Ammo = 6;
      MaxAmmo = 6;
      AmmoReloadRate = 5;
      AmmoReloadRateRandom = 0;
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

      AngularRange = 25;
    }
  }
}
