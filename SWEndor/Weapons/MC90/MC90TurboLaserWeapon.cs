using MTV3D65;
using SWEndor.ActorTypes;

namespace SWEndor.Weapons.Types
{
  public class MC90TurboLaserWeapon : WeaponInfo
  {
    public MC90TurboLaserWeapon() : base("MC90 Laser")
    {
      WeaponProjectile = RedLaserATI.Instance();
      WeaponCooldownRate = 2.4f;
      WeaponCooldownRateRandom = 0;

      MaxAmmo = 8;
      AmmoReloadRate = 10;
      AmmoReloadRateRandom = 10;
      AmmoReloadAmount = 2;

      FirePositions = new TV_3DVECTOR[] { new TV_3DVECTOR(0, 0, 0)
                                        };

      // Auto Aim Bot
      EnablePlayerAutoAim = true;
      EnableAIAutoAim = true;
      AutoAimMinDeviation = 0.5f;
      AutoAimMaxDeviation = 2.5f;

      // Player Config
      RequirePlayerTargetLock = false;

      AngularRange = 270;
    }
  }
}
