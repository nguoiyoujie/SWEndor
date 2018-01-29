using MTV3D65;
using System;
using System.Collections.Generic;
using System.Text;

namespace SWEndor
{
  public class AcclamatorTurboLaserWeapon : WeaponInfo
  {
    public AcclamatorTurboLaserWeapon() : base("Acclamator Laser")
    {
      WeaponProjectile = YellowLaserATI.Instance();
      WeaponCooldownRate = 1.5f;
      WeaponCooldownRateRandom = 0;

      MaxAmmo = 4;
      AmmoReloadRate = 9f;
      AmmoReloadRateRandom = 15f;
      AmmoReloadAmount = 4;

      FirePositions = new TV_3DVECTOR[] { new TV_3DVECTOR(0, 0, 0)
                                        };

      // Auto Aim Bot
      EnablePlayerAutoAim = true;
      EnableAIAutoAim = true;
      AutoAimMinDeviation = 0.65f;
      AutoAimMaxDeviation = 1.85f;

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
