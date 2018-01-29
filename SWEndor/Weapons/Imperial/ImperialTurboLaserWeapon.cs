using MTV3D65;
using System;
using System.Collections.Generic;
using System.Text;

namespace SWEndor
{
  public class ImperialTurboLaserWeapon : WeaponInfo
  {
    public ImperialTurboLaserWeapon() : base("Imperial Laser")
    {
      WeaponProjectile = YellowLaserATI.Instance();
      WeaponCooldownRate = 2.75f;
      WeaponCooldownRateRandom = 0;

      MaxAmmo = 4;
      AmmoReloadRate = 12;
      AmmoReloadRateRandom = 24;
      AmmoReloadAmount = 2;

      FirePositions = new TV_3DVECTOR[] { new TV_3DVECTOR(0, 0, 0)
                                        };

      // Auto Aim Bot
      EnablePlayerAutoAim = true;
      EnableAIAutoAim = true;
      AutoAimMinDeviation = 0.2f;
      AutoAimMaxDeviation = 3f;

      // Player Config
      RequirePlayerTargetLock = false;

      // AI Config
      AIAttackFighters = true;
      AIAttackShips = true;
      AIAttackAddons = true;
      AIAttackNull = true;

      // 
      FireSound = "Laser_sf";

      AngularRange = 270;
    }
  }
}
