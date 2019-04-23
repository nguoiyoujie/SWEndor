﻿using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;

namespace SWEndor.Weapons.Types
{
  public class MissileBoat_MissileWeapon : WeaponInfo
  {
    public MissileBoat_MissileWeapon() : base("Missile Boat Missile")
    {
      WeaponProjectile = MissileATI.Instance();
      WeaponCooldownRate = 0.25f;
      WeaponCooldownRateRandom = 0;

      Ammo = 40;
      MaxAmmo = 40;
      AmmoReloadRate = 20;
      AmmoReloadRateRandom = 15;
      AmmoReloadAmount = 5;

      FirePositions = new TV_3DVECTOR[] { new TV_3DVECTOR(15, 0, 50)
                                        , new TV_3DVECTOR(-15, 0, 50)
                                        , new TV_3DVECTOR(15, 10, 50)
                                        , new TV_3DVECTOR(-15, 10, 50)
                                        };

      // Auto Aim Bot
      EnablePlayerAutoAim = true;
      EnableAIAutoAim = true;
      ProjectileWaitBeforeHoming = 0.8f;

      // Player Config
      RequirePlayerTargetLock = true;

      // AI Config
      AIAttackTargets = TargetType.SHIP | TargetType.ADDON;
      AIAttackNull = false;

      // 
      FireSound = "torpedo";

      AngularRange = 5;
    }
  }
}