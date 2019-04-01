using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;

namespace SWEndor.Weapons.Types
{
  public class ImperialAntiShipMissileWeapon : WeaponInfo
  {
    public ImperialAntiShipMissileWeapon() : base("Imperial Anti-Ship Missile")
    {
      WeaponProjectile = MissileATI.Instance();
      WeaponCooldownRate = 0.5f;
      WeaponCooldownRateRandom = 0;

      MaxAmmo = 3;
      AmmoReloadRate = 40;
      AmmoReloadRateRandom = 25;
      AmmoReloadAmount = 3;
      ProjectileWaitBeforeHoming = 1.2f;

      FirePositions = new TV_3DVECTOR[] { new TV_3DVECTOR(0, 0, 0)
                                        };

      // Auto Aim Bot
      EnablePlayerAutoAim = false;
      EnableAIAutoAim = false;

      // Player Config
      RequirePlayerTargetLock = true;

      // AI Config
      AIAttackTargets = TargetType.SHIP | TargetType.ADDON;
      AIAttackNull = false;

      AngularRange = 180;
      Range = 17500;
    }
  }
}
