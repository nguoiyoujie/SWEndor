using MTV3D65;
using SWEndor.Actors.Types;

namespace SWEndor.Weapons.Types
{
  public class TIE_LN_LaserWeapon : WeaponInfo
  {
    public TIE_LN_LaserWeapon() : base("TIE/LN Laser")
    {
      WeaponProjectile = GreenLaserATI.Instance();
      WeaponCooldownRate = 0.375f;
      WeaponCooldownRateRandom = 0;

      //FirePositions = new TV_3DVECTOR[] { new TV_3DVECTOR(0, 0, 50)
      //                                  };

      FirePositions = new TV_3DVECTOR[] { new TV_3DVECTOR(-8, 0, 50)
                                        , new TV_3DVECTOR(8, 0, 50)
                                        };

      // Auto Aim Bot
      EnablePlayerAutoAim = false;
      EnableAIAutoAim = false;

      // Player Config
      RequirePlayerTargetLock = false;
    }
  }
}
