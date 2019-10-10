using MTV3D65;

namespace SWEndor.Weapons.Types
{
  public class NullWeapon : WeaponInfo
  {
    public static readonly NullWeapon Instance = new NullWeapon();

    private NullWeapon() : base("Null")
    {
      //WeaponCooldown = Globals.Engine.Game.GameTime + 9999999;
      WeaponCooldownRate = 999999999;
      WeaponCooldownRateRandom = 0;

      MaxAmmo = 0;
      AmmoReloadRate = 1000;
      AmmoReloadAmount = 0;

      FirePositions = new TV_3DVECTOR[] { new TV_3DVECTOR(0, 0, 0)
                                        };
      // Auto Aim Bot
      EnablePlayerAutoAim = true;
      EnableAIAutoAim = true;

      // Player Config
      RequirePlayerTargetLock = false;

      AngularRange = 0;
    }
  }
}
