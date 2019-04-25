using MTV3D65;
using SWEndor.ActorTypes.Instances;

namespace SWEndor.Weapons.Types
{
  public class FalconGunPortLaserWeapon : WeaponInfo
  {
    public FalconGunPortLaserWeapon() : base("Falcon Gun-Port Laser", "Red Laser")
    {
      WeaponCooldownRate = 0.1f;
      WeaponCooldownRateRandom = 0;

      MaxAmmo = 6;
      AmmoReloadRate = 8;
      AmmoReloadRateRandom = 7;
      AmmoReloadAmount = 6;

      FirePositions = new TV_3DVECTOR[] { new TV_3DVECTOR(0, 0, 0)
                                        };

      // Auto Aim Bot
      EnablePlayerAutoAim = true;
      EnableAIAutoAim = true;
      AutoAimMinDeviation = 0.75f;
      AutoAimMaxDeviation = 1.25f;

      // Player Config
      RequirePlayerTargetLock = false;

      AngularRange = 270;
    }
  }
}
