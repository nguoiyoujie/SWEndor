using MTV3D65;
using SWEndor.ActorTypes.Instances;

namespace SWEndor.Weapons.Types
{
  public class MissileBoat_LaserWeapon : WeaponInfo
  {
    public MissileBoat_LaserWeapon() : base("Missile Boat Laser", "Green Laser")
    {
      WeaponCooldownRate = 0.24f;
      WeaponCooldownRateRandom = 0;

      FirePositions = new TV_3DVECTOR[] { new TV_3DVECTOR(0, -15, 50) };

      // Auto Aim Bot
      EnablePlayerAutoAim = false;
      EnableAIAutoAim = true;

      // Player Config
      RequirePlayerTargetLock = false;

      AngularRange = 5;
    }
  }
}
