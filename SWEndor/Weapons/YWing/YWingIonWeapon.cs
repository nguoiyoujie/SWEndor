using MTV3D65;
using System;
using System.Collections.Generic;
using System.Text;

namespace SWEndor
{
  public class YWingIonWeapon : WeaponInfo
  {
    public YWingIonWeapon() : base("Y-Wing Ion")
    {
      WeaponProjectile = SmallIonLaserATI.Instance();
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
      AIAttackFighters = false;
      AIAttackShips = true;
      AIAttackAddons = true;
      AIAttackNull = false;

      // 
      FireSound = "Laser_sf";

      Range = 3000;
    }
  }
}
