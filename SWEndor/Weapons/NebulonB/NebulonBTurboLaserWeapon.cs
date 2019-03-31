using MTV3D65;
using SWEndor.Actors.Types;

namespace SWEndor.Weapons.Types
{
  public class NebulonBTurboLaserWeapon : WeaponInfo
  {
    public NebulonBTurboLaserWeapon() : base("Nebulon-B Laser")
    {
      WeaponProjectile = RedLaserATI.Instance();
      WeaponCooldownRate = 3.5f;
      WeaponCooldownRateRandom = 0;

      MaxAmmo = 4;
      AmmoReloadRate = 8;
      AmmoReloadRateRandom = 8;
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
