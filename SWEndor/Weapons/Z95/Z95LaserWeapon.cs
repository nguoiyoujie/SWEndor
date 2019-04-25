using MTV3D65;

namespace SWEndor.Weapons.Types
{
  public class Z95LaserWeapon : WeaponInfo
  {
    public Z95LaserWeapon() : base("Z-95 Laser", "Red Laser")
    { 
      WeaponCooldownRate = 0.21f;
      WeaponCooldownRateRandom = 0;
      
      FirePositions = new TV_3DVECTOR[] { new TV_3DVECTOR(20, 2, 40)
                                        , new TV_3DVECTOR(-20, 2, 40)
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
