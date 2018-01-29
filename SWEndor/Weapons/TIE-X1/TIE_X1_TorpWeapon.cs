using MTV3D65;
using System;
using System.Collections.Generic;
using System.Text;

namespace SWEndor
{
  public class TIE_X1_TorpWeapon : WeaponInfo
  {
    public TIE_X1_TorpWeapon() : base("TIE/X1 Torpedo")
    {
      WeaponProjectile = MissileATI.Instance();
      WeaponCooldownRate = 0.15f;
      WeaponCooldownRateRandom = 0;

      Ammo = 6;
      MaxAmmo = 6;
      AmmoReloadRate = 5;
      AmmoReloadRateRandom = 0;
      AmmoReloadAmount = 1;

      FirePositions = new TV_3DVECTOR[] { new TV_3DVECTOR(0, -10, 100)
                                        };

      // Auto Aim Bot
      EnablePlayerAutoAim = true;
      EnableAIAutoAim = true;

      // Player Config
      RequirePlayerTargetLock = true;

      // AI Config
      AIAttackFighters = false;
      AIAttackShips = true;
      AIAttackAddons = true;
      AIAttackNull = false;

      // 
      FireSound = "torpedo";

      AngularRange = 25;
    }
  }
}
