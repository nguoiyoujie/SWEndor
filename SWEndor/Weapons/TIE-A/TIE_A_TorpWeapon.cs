using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;

namespace SWEndor.Weapons.Types
{
  public class TIE_A_TorpWeapon : WeaponInfo
  {
    public TIE_A_TorpWeapon() : base("TIE/A Torpedo")
    {
      WeaponProjectile = TorpedoATI.Instance();
      WeaponCooldownRate = 0.3f;
      WeaponCooldownRateRandom = 0;

      Ammo = 2;
      MaxAmmo = 2;
      AmmoReloadRate = 8;
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

      AngularRange = 5;
    }
  }
}
