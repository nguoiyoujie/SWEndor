using MTV3D65;
using System;
using System.Collections.Generic;
using System.Text;

namespace SWEndor
{
  public class ImperialAntiShipTurboLaserWeapon : WeaponInfo
  {
    public ImperialAntiShipTurboLaserWeapon() : base("Imperial Anti-Ship Laser")
    {
      WeaponProjectile = GreenAntiShipLaserATI.Instance();
      WeaponCooldownRate = 2.25f;
      WeaponCooldownRateRandom = 0;

      MaxAmmo = 4;
      AmmoReloadRate = 20;
      AmmoReloadRateRandom = 20;
      AmmoReloadAmount = 2;

      FirePositions = new TV_3DVECTOR[] { new TV_3DVECTOR(0, 0, 0)
                                        };

      // Auto Aim Bot
      EnablePlayerAutoAim = true;
      EnableAIAutoAim = true;
      AutoAimMinDeviation = 0.1f;
      AutoAimMaxDeviation = 4f;

      // Player Config
      RequirePlayerTargetLock = false;

      // AI Config
      AIAttackFighters = false;
      AIAttackShips = true;
      AIAttackAddons = false;
      AIAttackNull = false;

      // 
      FireSound = "Laser_sf";

      AngularRange = 270;
      Range = 12500;
    }
  }
}
