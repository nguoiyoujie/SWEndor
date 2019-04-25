using MTV3D65;
using SWEndor.ActorTypes.Instances;

namespace SWEndor.Weapons.Types
{
  public class TIE_IN_LaserWeapon : WeaponInfo
  {
    public TIE_IN_LaserWeapon() : base("TIE/IN Laser", "Green Laser")
    {
      WeaponCooldownRate = 0.15f;
      WeaponCooldownRateRandom = 0;

      FirePositions = new TV_3DVECTOR[] { new TV_3DVECTOR(7, 5, 50)
                                        , new TV_3DVECTOR(-7, -5, 50)
                                        , new TV_3DVECTOR(7, -5, 50)
                                        , new TV_3DVECTOR(-7, 5, 50)
                                        };

      // Auto Aim Bot
      EnablePlayerAutoAim = false;
      EnableAIAutoAim = false;

      // Player Config
      RequirePlayerTargetLock = false;
    }
  }
}
