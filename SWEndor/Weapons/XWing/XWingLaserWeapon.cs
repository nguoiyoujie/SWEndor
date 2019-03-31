using MTV3D65;
using SWEndor.Actors.Types;

namespace SWEndor.Weapons.Types
{
  public class XWingLaserWeapon : WeaponInfo
  {
    public XWingLaserWeapon() : base("X-Wing Laser")
    {
      WeaponProjectile = RedLaserATI.Instance();
      WeaponCooldownRate = 0.185f;
      WeaponCooldownRateRandom = 0;
      
      FirePositions = new TV_3DVECTOR[] { new TV_3DVECTOR(20, 5, 40)
                                        , new TV_3DVECTOR(-20, -7, 40)
                                        , new TV_3DVECTOR(20, -7, 40)
                                        , new TV_3DVECTOR(-20, 5, 40)
                                        };

      // Auto Aim Bot
      EnablePlayerAutoAim = false;
      EnableAIAutoAim = true;
      AutoAimMinDeviation = 1;
      AutoAimMaxDeviation = 1;

      // Player Config
      RequirePlayerTargetLock = false;
    }
  }
}
