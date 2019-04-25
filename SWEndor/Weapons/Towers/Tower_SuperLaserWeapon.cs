using MTV3D65;
using SWEndor.ActorTypes.Instances;

namespace SWEndor.Weapons.Types
{
  public class Tower_SuperLaserWeapon : WeaponInfo
  {
    public Tower_SuperLaserWeapon() : base("Tower Super Laser", "Green Laser")
    {
      WeaponCooldownRate = 0.18f;
      WeaponCooldownRateRandom = 0;

      FirePositions = new TV_3DVECTOR[] { new TV_3DVECTOR(-8, 0, 25)
                                        , new TV_3DVECTOR(8, 0, 25)
                                        };

      // Auto Aim Bot
      EnablePlayerAutoAim = false;
      EnableAIAutoAim = false;

      // Player Config
      RequirePlayerTargetLock = false;
    }
  }
}
