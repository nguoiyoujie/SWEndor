using MTV3D65;
using SWEndor.ActorTypes;

namespace SWEndor.Weapons.Types
{
  public class TIE_X1_LaserWeapon : WeaponInfo
  {
    public TIE_X1_LaserWeapon() : base("TIE/X1 Laser")
    {
      WeaponProjectile = GreenLaserAdvancedATI.Instance();
      WeaponCooldownRate = 0.075f;
      WeaponCooldownRateRandom = 0;

      FirePositions = new TV_3DVECTOR[] { new TV_3DVECTOR(15, 0, 50)
                                        , new TV_3DVECTOR(-15, 0, 50)
                                        , new TV_3DVECTOR(5, 0, 50)
                                        , new TV_3DVECTOR(-5, 0, 50)
                                        };

      // Auto Aim Bot
      EnablePlayerAutoAim = true;
      EnableAIAutoAim = true;

      // Player Config
      RequirePlayerTargetLock = false;

      AngularRange = 20;
    }
  }
}
