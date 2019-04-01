using MTV3D65;
using SWEndor.ActorTypes;

namespace SWEndor.Weapons.Types
{
  public class TIE_IN_DblLaserWeapon : WeaponInfo
  {
    public TIE_IN_DblLaserWeapon() : base("TIE/IN Double Laser")
    {
      WeaponProjectile = Green2LaserATI.Instance();
      WeaponCooldownRate = 0.3f;
      WeaponCooldownRateRandom = 0;

      FirePositions = new TV_3DVECTOR[] { new TV_3DVECTOR(0, -10, 50)
                                        , new TV_3DVECTOR(0, 10, 50)
                                        };

      //FirePositions = new TV_3DVECTOR[] { new TV_3DVECTOR(-8, 0, 50)
      //                                  , new TV_3DVECTOR(8, 0, 50)
      //                                  };

      // Auto Aim Bot
      EnablePlayerAutoAim = false;
      EnableAIAutoAim = false;

      // Player Config
      RequirePlayerTargetLock = false;

      AngularRange = 2.5f;
    }
  }
}
