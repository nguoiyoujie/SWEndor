using MTV3D65;
using SWEndor.ActorTypes.Instances;

namespace SWEndor.Weapons.Types
{
  public class TIE_LN_DblLaserWeapon : WeaponInfo
  {
    public TIE_LN_DblLaserWeapon() : base("TIE/LN Double Laser")
    {
      WeaponProjectile = Green2LaserATI.Instance();
      WeaponCooldownRate = 0.75f;
      WeaponCooldownRateRandom = 0;

      FirePositions = new TV_3DVECTOR[] { new TV_3DVECTOR(0, 0, 50)
                                        };

      //FirePositions = new TV_3DVECTOR[] { new TV_3DVECTOR(-8, 0, 50)
      //                                  , new TV_3DVECTOR(8, 0, 50)
      //                                  };

      // Auto Aim Bot
      EnablePlayerAutoAim = false;
      EnableAIAutoAim = false;

      // Player Config
      RequirePlayerTargetLock = false;
   }
  }
}
