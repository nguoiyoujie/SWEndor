using MTV3D65;
using System;
using System.Collections.Generic;
using System.Text;

namespace SWEndor
{
  public class CorellianTurboLaserWeapon : WeaponInfo
  {
    public CorellianTurboLaserWeapon() : base("Corellian Laser")
    {
      WeaponProjectile = RedLaserATI.Instance();
      WeaponCooldownRate = 2.25f;
      WeaponCooldownRateRandom = 0;

      MaxAmmo = 4;
      AmmoReloadRate = 7.5f;
      AmmoReloadRateRandom = 7.5f;
      AmmoReloadAmount = 2;

      FirePositions = new TV_3DVECTOR[] { new TV_3DVECTOR(0, 0, 0)
                                        };

      // Auto Aim Bot
      EnablePlayerAutoAim = true;
      EnableAIAutoAim = true;
      AutoAimMinDeviation = 0.85f;
      AutoAimMaxDeviation = 1.75f;

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
