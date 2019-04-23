using MTV3D65;
using SWEndor.ActorTypes.Instances;

namespace SWEndor.Weapons.Types
{
  public class CorellianTurboLaserWeapon : WeaponInfo
  {
    public CorellianTurboLaserWeapon() : base("Corellian Laser")
    {
      WeaponProjectile = RedLaserATI.Instance();
      WeaponCooldownRate = 2.25f;
      WeaponCooldownRateRandom = 0;

      MaxAmmo = 4;
      AmmoReloadRate = 7.5f;
      AmmoReloadRateRandom = 7.5f;
      AmmoReloadAmount = 2;

      FirePositions = new TV_3DVECTOR[] { new TV_3DVECTOR(0, 0, 0)
                                        };

      // Auto Aim Bot
      EnablePlayerAutoAim = true;
      EnableAIAutoAim = true;
      AutoAimMinDeviation = 0.85f;
      AutoAimMaxDeviation = 1.75f;

      // Player Config
      RequirePlayerTargetLock = false;

      AngularRange = 270;
    }
  }
}
