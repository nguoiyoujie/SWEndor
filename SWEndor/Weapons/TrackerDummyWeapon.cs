﻿using MTV3D65;
using SWEndor.ActorTypes.Instances;

namespace SWEndor.Weapons.Types
{
  public class TrackerDummyWeapon : WeaponInfo
  {
    public TrackerDummyWeapon() : base("Tracker Dummy")
    {
      WeaponProjectile = RedLaserATI.Instance();
      WeaponCooldown = Globals.Engine.Game.GameTime + 9999999;
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

      AngularRange = 0;
    }
  }
}
