﻿using MTV3D65;

namespace SWEndor.Weapons.Types
{
  public class TIE_A_LaserWeapon : WeaponInfo
  {
    public TIE_A_LaserWeapon() : base("TIE/A Laser", "Green Laser Advanced")
    {
      WeaponCooldownRate = 0.18f;
      WeaponCooldownRateRandom = 0;

      FirePositions = new TV_3DVECTOR[] { new TV_3DVECTOR(15, 5, 50)
                                        , new TV_3DVECTOR(-15, 5, 50)
                                        , new TV_3DVECTOR(5, -5, 50)
                                        , new TV_3DVECTOR(-5, -5, 50)
                                        };

      // Auto Aim Bot
      EnablePlayerAutoAim = false;
      EnableAIAutoAim = true;

      // Player Config
      RequirePlayerTargetLock = false;

      AngularRange = 5;
    }
  }
}
