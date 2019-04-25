using MTV3D65;

namespace SWEndor.Weapons.Types
{
  public class DeathStarLaserWeapon : WeaponInfo
  {
    public DeathStarLaserWeapon() : base("Death Star Laser", "Death Star Laser")
    {
      WeaponCooldownRate = 0.01f;
      WeaponCooldownRateRandom = 0;

      MaxAmmo = -1;

      FirePositions = new TV_3DVECTOR[] { new TV_3DVECTOR(0, 0, 0)
                                        };

      // Auto Aim Bot
      EnablePlayerAutoAim = true;
      EnableAIAutoAim = true;

      // Player Config
      RequirePlayerTargetLock = false;
    }
  }
}
