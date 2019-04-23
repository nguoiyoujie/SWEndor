using MTV3D65;
using SWEndor.ActorTypes;
using SWEndor.ActorTypes.Instances;

namespace SWEndor.Weapons.Types
{
  public class TIE_sa_TorpWeapon : WeaponInfo
  {
    public TIE_sa_TorpWeapon() : base("TIE/sa Torpedo")
    {
      WeaponProjectile = TorpedoATI.Instance();
      WeaponCooldownRate = 0.3f;
      WeaponCooldownRateRandom = 0;

      Ammo = 8;
      MaxAmmo = 8;
      AmmoReloadRate = 5;
      AmmoReloadRateRandom = 5;
      AmmoReloadAmount = 1;

      FirePositions = new TV_3DVECTOR[] { new TV_3DVECTOR(8, -10, 70)
                                        , new TV_3DVECTOR(-8, -10, 70)
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
