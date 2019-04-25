using MTV3D65;
using SWEndor.ActorTypes;
using SWEndor.ActorTypes.Instances;

namespace SWEndor.Weapons.Types
{
  public class BWingTorpWeapon : WeaponInfo
  {
    public BWingTorpWeapon() : base("B-Wing Torpedo", "Torpedo")
    {
      WeaponCooldownRate = 0.25f;
      WeaponCooldownRateRandom = 0;

      Ammo = 6;
      MaxAmmo = 6;
      AmmoReloadRate = 10;
      AmmoReloadRateRandom = 10;
      AmmoReloadAmount = 1;

      FirePositions = new TV_3DVECTOR[] { new TV_3DVECTOR(0, -30, 100)
                                        , new TV_3DVECTOR(0, -50, 100)
                                        , new TV_3DVECTOR(0, -70, 100)
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
