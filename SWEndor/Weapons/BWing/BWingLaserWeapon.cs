using MTV3D65;
using SWEndor.Actors.Types;

namespace SWEndor.Weapons.Types
{
  public class BWingLaserWeapon : WeaponInfo
  {
    public BWingLaserWeapon() : base("B-Wing Laser")
    {
      WeaponProjectile = RedLaserATI.Instance();
      WeaponCooldownRate = 0.2f;
      WeaponCooldownRateRandom = 0;

      FirePositions = new TV_3DVECTOR[] { new TV_3DVECTOR(0, 2, 50)
                                        , new TV_3DVECTOR(0, -80, 50)
                                        , new TV_3DVECTOR(25, -30, 50)
                                        , new TV_3DVECTOR(-25, -30, 50)
                                        };

      // Auto Aim Bot
      EnablePlayerAutoAim = false;
      EnableAIAutoAim = true;

      // Player Config
      RequirePlayerTargetLock = false;
    }
  }
}
