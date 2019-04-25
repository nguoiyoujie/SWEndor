using MTV3D65;
using SWEndor.ActorTypes;

namespace SWEndor.Weapons.Types
{
  public class MissileBoat_TorpWeapon : WeaponInfo
  {
    public MissileBoat_TorpWeapon() : base("Missile Boat Torpedo", "Torpedo")
    {
      WeaponCooldownRate = 0.4f;
      WeaponCooldownRateRandom = 0;

      Ammo = 20;
      MaxAmmo = 20;
      AmmoReloadRate = 10;
      AmmoReloadRateRandom = 10;
      AmmoReloadAmount = 2;

      FirePositions = new TV_3DVECTOR[] { new TV_3DVECTOR(15, 0, 50)
                                        , new TV_3DVECTOR(-15, 0, 50)
                                        , new TV_3DVECTOR(15, 10, 50)
                                        , new TV_3DVECTOR(-15, 10, 50)
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
