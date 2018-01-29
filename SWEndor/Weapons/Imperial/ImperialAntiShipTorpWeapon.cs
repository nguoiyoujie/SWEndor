using MTV3D65;
using System;
using System.Collections.Generic;
using System.Text;

namespace SWEndor
{
  public class ImperialAntiShipTorpWeapon : WeaponInfo
  {
    public ImperialAntiShipTorpWeapon() : base("Imperial Anti-Ship Torpedo")
    {
      WeaponProjectile = MissileATI.Instance();
      WeaponCooldownRate = 5f;
      WeaponCooldownRateRandom = 0;

      MaxAmmo = 2;
      AmmoReloadRate = 40;
      AmmoReloadRateRandom = 25;
      AmmoReloadAmount = 2;

      FirePositions = new TV_3DVECTOR[] { new TV_3DVECTOR(0, 0, 0)
                                        };

      // Auto Aim Bot
      EnablePlayerAutoAim = true;
      EnableAIAutoAim = true;

      // Player Config
      RequirePlayerTargetLock = true;

      // AI Config
      AIAttackFighters = false;
      AIAttackShips = true;
      AIAttackAddons = false;
      AIAttackNull = false;

      // 
      FireSound = "torpedo";

      AngularRange = 270;
      Range = 17500;
    }
  }
}
