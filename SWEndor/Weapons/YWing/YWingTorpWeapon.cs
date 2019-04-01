using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;

namespace SWEndor.Weapons.Types
{
  public class YWingTorpWeapon : WeaponInfo
  {
    public YWingTorpWeapon() : base("Y-Wing Torpedo")
    {
      WeaponProjectile = TorpedoATI.Instance();
      WeaponCooldownRate = 0.15f;
      WeaponCooldownRateRandom = 0;

      Ammo = 10;
      MaxAmmo = 10;
      AmmoReloadRate = 8;
      AmmoReloadRateRandom = 4;
      AmmoReloadAmount = 1;

      FirePositions = new TV_3DVECTOR[] { new TV_3DVECTOR(0, -10, 100)
                                        , new TV_3DVECTOR(-5, -10, 100)
                                        , new TV_3DVECTOR(5, -10, 100)
                                        , new TV_3DVECTOR(-10, -10, 100)
                                        , new TV_3DVECTOR(10, -10, 100)
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
