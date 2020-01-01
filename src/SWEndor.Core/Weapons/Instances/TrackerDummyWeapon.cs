using Primrose.Primitives.ValueTypes;

namespace SWEndor.Weapons.Types
{
  public class TrackerDummyWeapon : WeaponInfo
  {
    public static readonly TrackerDummyWeapon Instance = new TrackerDummyWeapon();

    private TrackerDummyWeapon() : base("Tracker")
    {
      Port.CooldownRate = new float2(999999999, 0);

      Ammo.Max = 1;
      Ammo.ReloadRate = new float2 (1000, 0);
      Ammo.ReloadAmount = 1;

      Port.FirePos = new float3[] { new float3(0, 0, 0) };

      // Auto Aim Bot
      Aim.EnablePlayerAutoAim = true;
      Aim.EnableAIAutoAim = true;

      // Player Config
      Targeter.RequirePlayerTargetLock = false;

      Targeter.AngularRange = 0;
    }
  }
}
