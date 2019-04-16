using MTV3D65;
using SWEndor.ActorTypes;

namespace SWEndor.Weapons.Types
{
  public class TIE_A_LaserWeapon : WeaponInfo
  {
    public TIE_A_LaserWeapon() : base("TIE/A Laser")
    {
      WeaponProjectile = GreenLaserAdvancedATI.Instance();
      WeaponCooldownRate = 0.18f;
      WeaponCooldownRateRandom = 0;

      FirePositions = new TV_3DVECTOR[] { new TV_3DVECTOR(15, 0, 50)
                                        , new TV_3DVECTOR(-15, 0, 50)
                                        , new TV_3DVECTOR(5, 0, 50)
                                        , new TV_3DVECTOR(-5, 0, 50)
                                        };

      // Auto Aim Bot
      EnablePlayerAutoAim = false;
      EnableAIAutoAim = true;

      // Player Config
      RequirePlayerTargetLock = false;

      AngularRange = 5;
    }
  }
}
