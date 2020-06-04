using Primrose.Primitives.ValueTypes;

namespace SWEndor.Weapons.Types
{
  public class NullWeapon : WeaponInfo
  {
    public static readonly NullWeapon Instance = new NullWeapon();

    private NullWeapon() : base("Null")
    {
      Port.CooldownRate = new float2(999999999, 0);

      Ammo.Max = 0;
      Ammo.ReloadRate = new float2(1000, 0);
      Ammo.ReloadAmount = 0;

      Port.FirePositions = new float3[] { new float3(0, 0, 0) };
      // Auto Aim Bot
      Aim.EnablePlayerAutoAim = true;
      Aim.EnableAIAutoAim = true;

      // Player Config
      Targeter.RequirePlayerTargetLock = false;

      Targeter.AngularRange = 0;
    }
  }
}
