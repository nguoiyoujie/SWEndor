using Primrose.Primitives.ValueTypes;

namespace SWEndor.Game.Weapons.Types
{
  public class TrackerDummyWeapon : WeaponInfo
  {
    public static readonly TrackerDummyWeapon Instance = new TrackerDummyWeapon();

    private TrackerDummyWeapon() : base("Tracker")
    {
      Port.CooldownRate = 999999999;
      Port.CooldownRateRandom = 0;

      Ammo.Max = 1;
      Ammo.ReloadRate = 1000;
      Ammo.ReloadRateRandom = 0;
      Ammo.ReloadAmount = 1;

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
