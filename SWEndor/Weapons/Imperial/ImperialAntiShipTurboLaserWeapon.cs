using MTV3D65;
using SWEndor.ActorTypes;
using SWEndor.ActorTypes.Instances;

namespace SWEndor.Weapons.Types
{
  public class ImperialAntiShipTurboLaserWeapon : WeaponInfo
  {
    public ImperialAntiShipTurboLaserWeapon() : base("Imperial Anti-Ship Laser", "Green Anti-Ship Laser")
    {
      WeaponCooldownRate = 2.25f;
      WeaponCooldownRateRandom = 0;

      MaxAmmo = 4;
      AmmoReloadRate = 20;
      AmmoReloadRateRandom = 20;
      AmmoReloadAmount = 2;

      FirePositions = new TV_3DVECTOR[] { new TV_3DVECTOR(0, 0, 0)
                                        };

      // Auto Aim Bot
      EnablePlayerAutoAim = true;
      EnableAIAutoAim = true;
      AutoAimMinDeviation = 0.1f;
      AutoAimMaxDeviation = 4f;

      // Player Config
      RequirePlayerTargetLock = false;

      // AI Config
      AIAttackTargets = TargetType.SHIP | TargetType.ADDON;
      AIAttackNull = false;

      AngularRange = 270;
      Range = 12500;
    }
  }
}
