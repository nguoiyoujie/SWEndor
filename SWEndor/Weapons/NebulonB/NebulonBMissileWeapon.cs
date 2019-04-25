using MTV3D65;
using SWEndor.ActorTypes;
using SWEndor.ActorTypes.Instances;

namespace SWEndor.Weapons.Types
{
  public class NebulonBMissileWeapon : WeaponInfo
  {
    public NebulonBMissileWeapon() : base("Nebulon B Missile", "Missile")
    {
      WeaponCooldownRate = 0.5f;
      WeaponCooldownRateRandom = 0;

      Ammo = 3;
      MaxAmmo = 3;
      AmmoReloadRate = 40;
      AmmoReloadRateRandom = 15;
      AmmoReloadAmount = 3;
      ProjectileWaitBeforeHoming = 1.2f;

      FirePositions = new TV_3DVECTOR[] { new TV_3DVECTOR(0, 0, 0)
                                        };

      // Auto Aim Bot
      EnablePlayerAutoAim = false;
      EnableAIAutoAim = false;

      // Player Config
      RequirePlayerTargetLock = false;

      // AI Config
      AIAttackTargets = TargetType.SHIP | TargetType.ADDON;
      AIAttackNull = false;

      AngularRange = 180;
      Range = 17500;
    }
  }
}
