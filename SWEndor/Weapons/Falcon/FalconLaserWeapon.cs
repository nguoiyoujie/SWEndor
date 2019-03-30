using MTV3D65;
using SWEndor.Actors.Types;

namespace SWEndor.Weapons.Types
{
  public class FalconLaserWeapon : WeaponInfo
  {
    public FalconLaserWeapon() : base("Falcon Laser")
    {
      WeaponProjectile = RedLaserATI.Instance();
      WeaponCooldownRate = 0.12f;
      WeaponCooldownRateRandom = 0;

      FirePositions = new TV_3DVECTOR[] { new TV_3DVECTOR(0, 7, 100)
                                        };

      // Auto Aim Bot
      EnablePlayerAutoAim = false;
      EnableAIAutoAim = true;
      AutoAimMinDeviation = 0.5f;
      AutoAimMaxDeviation = 1.5f;

      // Player Config
      RequirePlayerTargetLock = false;
    }
  }
}
