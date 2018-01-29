using MTV3D65;
using System;
using System.Collections.Generic;
using System.Text;

namespace SWEndor
{
  public class DeathStarLaserWeapon : WeaponInfo
  {
    public DeathStarLaserWeapon() : base("Death Star Laser")
    {
      WeaponProjectile = DeathStarLaserATI.Instance();
      WeaponCooldownRate = 0.01f;
      WeaponCooldownRateRandom = 0;

      MaxAmmo = -1;

      FirePositions = new TV_3DVECTOR[] { new TV_3DVECTOR(0, 0, 0)
                                        };

      // Auto Aim Bot
      EnablePlayerAutoAim = true;
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
