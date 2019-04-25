using MTV3D65;

namespace SWEndor.Weapons.Types
{
  public class TIE_D_LaserWeapon : WeaponInfo
  {
    public TIE_D_LaserWeapon() : base("TIE/D Laser", "Green Laser")
    {
      WeaponCooldownRate = 0.12f;
      WeaponCooldownRateRandom = 0;

      FirePositions = new TV_3DVECTOR[] { new TV_3DVECTOR(-11, 21, 50)
                                        , new TV_3DVECTOR(11, 21, 50)
                                        , new TV_3DVECTOR(24, -2, 50)
                                        , new TV_3DVECTOR(13, -19, 50)
                                        , new TV_3DVECTOR(-13, -19, 50)
                                        , new TV_3DVECTOR(-24, -2, 50)
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
