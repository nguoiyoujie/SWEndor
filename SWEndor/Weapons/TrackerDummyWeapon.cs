using MTV3D65;
using System;
using System.Collections.Generic;
using System.Text;

namespace SWEndor
{
  public class TrackerDummyWeapon : WeaponInfo
  {
    public TrackerDummyWeapon() : base("Tracker Dummy")
    {
      WeaponProjectile = RedLaserATI.Instance();
      WeaponCooldown = Game.Instance().GameTime + 9999999;
      WeaponCooldownRate = 1000;
      WeaponCooldownRateRandom = 0;

      MaxAmmo = 1;
      AmmoReloadRate = 1000;
      AmmoReloadAmount = 1;

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

      AngularRange = 0;
    }
  }
}
