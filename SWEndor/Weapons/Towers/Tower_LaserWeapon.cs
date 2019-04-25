using MTV3D65;

namespace SWEndor.Weapons.Types
{
  public class Tower_LaserWeapon : WeaponInfo
  {
    public Tower_LaserWeapon() : base("Tower Laser", "Green Double Laser")
    {
      WeaponCooldownRate = 1.15f;
      WeaponCooldownRateRandom = 0;

      /*
      MaxAmmo = 4;
      AmmoReloadRate = 6;
      AmmoReloadRateRandom = 6;
      AmmoReloadAmount = 4;
      */
      //FirePositions = new TV_3DVECTOR[] { new TV_3DVECTOR(0, 0, 50)
      //                                  };

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
