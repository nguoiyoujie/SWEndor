using MTV3D65;
using SWEndor.ActorTypes.Instances;

namespace SWEndor.Weapons.Types
{
  public class Tower_DblLaserWeapon : WeaponInfo
  {
    public Tower_DblLaserWeapon() : base("Tower Double Laser")
    {
      WeaponProjectile = Yellow2LaserATI.Instance();
      WeaponCooldownRate = 2.25f;
      WeaponCooldownRateRandom = 0;

      FirePositions = new TV_3DVECTOR[] { new TV_3DVECTOR(0, 0, 25)
                                        };

      // Auto Aim Bot
      EnablePlayerAutoAim = false;
      EnableAIAutoAim = false;

      // Player Config
      RequirePlayerTargetLock = false;
    }
  }
}
