using MTV3D65;
using System;
using System.Collections.Generic;
using System.Text;

namespace SWEndor
{
  public class TIE_sa_LaserWeapon : WeaponInfo
  {
    public TIE_sa_LaserWeapon() : base("TIE/sa Laser")
    {
      WeaponProjectile = GreenLaserATI.Instance();
      WeaponCooldownRate = 0.375f;
      WeaponCooldownRateRandom = 0;

      FirePositions = new TV_3DVECTOR[] { new TV_3DVECTOR(-10, 0, 50)
                                        , new TV_3DVECTOR(10, 0, 50)
                                        };

      // Auto Aim Bot
      EnablePlayerAutoAim = false;
      EnableAIAutoAim = true;

      // Player Config
      RequirePlayerTargetLock = false;

      // AI Config
      AIAttackFighters = true;
      AIAttackShips = true;
      AIAttackAddons = true;
      AIAttackNull = true;

      // 
      FireSound = "Laser_sf";
    }
  }
}
