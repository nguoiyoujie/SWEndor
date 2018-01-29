using MTV3D65;
using System;
using System.Collections.Generic;
using System.Text;

namespace SWEndor
{
  public class ExecutorTurboLaserWeapon : WeaponInfo
  {
    public ExecutorTurboLaserWeapon() : base("Executor Laser")
    {
      WeaponProjectile = YellowLaserATI.Instance();
      WeaponCooldownRate = 2.4f;
      WeaponCooldownRateRandom = 0;

      MaxAmmo = 8;
      AmmoReloadRate = 15f;
      AmmoReloadRateRandom = 25f;
      AmmoReloadAmount = 4;

      FirePositions = new TV_3DVECTOR[] { new TV_3DVECTOR(0, 0, 0)
                                        };

      // Auto Aim Bot
      EnablePlayerAutoAim = true;
      EnableAIAutoAim = true;
      AutoAimMinDeviation = 0.5f;
      AutoAimMaxDeviation = 2.5f;

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
