using MTV3D65;
using System;
using System.Collections.Generic;
using System.Text;

namespace SWEndor
{
  public class TIE_X1_LaserWeapon : WeaponInfo
  {
    public TIE_X1_LaserWeapon() : base("TIE/X1 Laser")
    {
      WeaponProjectile = GreenLaserAdvancedATI.Instance();
      WeaponCooldownRate = 0.075f;
      WeaponCooldownRateRandom = 0;

      FirePositions = new TV_3DVECTOR[] { new TV_3DVECTOR(15, 0, 50)
                                        , new TV_3DVECTOR(-15, 0, 50)
                                        , new TV_3DVECTOR(5, 0, 50)
                                        , new TV_3DVECTOR(-5, 0, 50)
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

      AngularRange = 20;
    }
  }
}
