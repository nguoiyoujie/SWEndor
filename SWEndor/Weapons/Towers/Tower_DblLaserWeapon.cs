using MTV3D65;

namespace SWEndor.Weapons.Types
{
  public class Tower_DblLaserWeapon : WeaponInfo
  {
    public Tower_DblLaserWeapon() : base("Tower Double Laser", "Yellow Double Laser")
    {
      WeaponCooldownRate = 2.25f;
      WeaponCooldownRateRandom = 0;

      FirePositions = new TV_3DVECTOR[] { new TV_3DVECTOR(0, 0, 25)
                                        };

      // Auto Aim Bot
      EnablePlayerAutoAim = false;
      EnableAIAutoAim = false;

      // Player Config
      RequirePlayerTargetLock = false;
    }
  }
}
