using MTV3D65;
using SWEndor.FileFormat.INI;
using SWEndor.Models;

namespace SWEndor.Weapons
{
  public class WeaponStatInfo
  {
    public WeaponStatInfo(string name)
    {
      Name = name;
    }

    public readonly string Name = "Null Weapon";
    public readonly string DisplayName = "null";

    public string WeaponProjectile = null;
    public bool IsActor = false;
    public float WeaponCooldown = 0;
    public float WeaponCooldownRate = 1;
    public float WeaponCooldownRateRandom = 0;

    public int Burst = 1;
    public int MaxAmmo = -1;
    public float AmmoReloadCooldown = 1;
    public float AmmoReloadRate = 1;
    public float AmmoReloadRateRandom = 0;
    public int AmmoReloadAmount = 1;
    public float ProjectileWaitBeforeHoming = 0;

    public TV_3DVECTOR[] FirePositions = null;
    public int CurrentPositionIndex = 0;

    // Auto Aim Bot
    public bool EnablePlayerAutoAim = false;
    public bool EnableAIAutoAim = false;
    public float AutoAimMinDeviation = 1;
    public float AutoAimMaxDeviation = 1;

    // Player Config
    public WeaponType Type = WeaponType.NONE;

    // Targeter
    public bool RequirePlayerTargetLock = false;
    public bool RequireAITargetLock = false;
    public float TargetLock_TimeRequired = 0; 

    public TargetAcqType PlayerTargetAcqType = TargetAcqType.ANY;
    public TargetAcqType AITargetAcqType = TargetAcqType.ENEMIES;

    // AI Config
    public TargetType AIAttackTargets = TargetType.ANY;

    public bool AIAttackNull = true;

    public float AngularRange = 10;
    public float Range = 0;

    public string[] FireSound = new string[] { "laser_sf" };

    public WeaponStatInfo(INIFile file, string sectionname)
    {
      Name = sectionname;
      DisplayName = file.GetString(sectionname, "DisplayName", DisplayName);

      WeaponProjectile = file.GetString(sectionname, "WeaponProjectile", WeaponProjectile);
      IsActor = file.GetBool(sectionname, "IsActor", IsActor);
      WeaponCooldown = file.GetFloat(sectionname, "WeaponCooldown", WeaponCooldown);
      WeaponCooldownRate = file.GetFloat(sectionname, "WeaponCooldownRate", WeaponCooldownRate);
      WeaponCooldownRateRandom = file.GetFloat(sectionname, "WeaponCooldownRateRandom", WeaponCooldownRateRandom);

      Burst = file.GetInt(sectionname, "Burst", Burst);
      MaxAmmo = file.GetInt(sectionname, "MaxAmmo", MaxAmmo);
      AmmoReloadCooldown = file.GetFloat(sectionname, "AmmoReloadCooldown", AmmoReloadCooldown);
      AmmoReloadRate = file.GetFloat(sectionname, "AmmoReloadRate", AmmoReloadRate);
      AmmoReloadRateRandom = file.GetFloat(sectionname, "AmmoReloadRateRandom", AmmoReloadRateRandom);
      AmmoReloadAmount = file.GetInt(sectionname, "AmmoReloadAmount", AmmoReloadAmount);
      ProjectileWaitBeforeHoming = file.GetFloat(sectionname, "ProjectileWaitBeforeHoming", ProjectileWaitBeforeHoming);

      float[] fpos = file.GetFloatArray(sectionname, "FirePositions", new float[0]);
      FirePositions = new TV_3DVECTOR[fpos.Length / 3];
      for (int p = 0; p + 2 < fpos.Length; p += 3)
        FirePositions[p / 3] = new TV_3DVECTOR(fpos[p], fpos[p + 1], fpos[p + 2]);

      CurrentPositionIndex = file.GetInt(sectionname, "CurrentPositionIndex", CurrentPositionIndex);

      // Auto Aim Bot
      EnablePlayerAutoAim = file.GetBool(sectionname, "EnablePlayerAutoAim", EnablePlayerAutoAim);
      EnableAIAutoAim = file.GetBool(sectionname, "EnableAIAutoAim", EnableAIAutoAim);
      AutoAimMinDeviation = file.GetFloat(sectionname, "AutoAimMinDeviation", AutoAimMinDeviation);
      AutoAimMaxDeviation = file.GetFloat(sectionname, "AutoAimMaxDeviation", AutoAimMaxDeviation);

      // Player Config
      Type = file.GetEnumValue(sectionname, "WeaponType", Type);

      // Targeter
      RequirePlayerTargetLock = file.GetBool(sectionname, "RequirePlayerTargetLock", RequirePlayerTargetLock);
      RequireAITargetLock = file.GetBool(sectionname, "RequireAITargetLock", RequireAITargetLock);
      TargetLock_TimeRequired = file.GetFloat(sectionname, "TargetLock_TimeRequired", TargetLock_TimeRequired);

      PlayerTargetAcqType = file.GetEnumValue(sectionname, "PlayerTargetAcqType ", PlayerTargetAcqType);
      AITargetAcqType = file.GetEnumValue(sectionname, "AITargetAcqType", AITargetAcqType);
      
      // AI Config
      AIAttackTargets = file.GetEnumValue(sectionname, "AIAttackTargets", AIAttackTargets);
      AIAttackNull = file.GetBool(sectionname, "AIAttackNull", AIAttackNull);

      AngularRange = file.GetFloat(sectionname, "AngularRange", AngularRange);
      Range = file.GetFloat(sectionname, "Range", Range);

      FireSound = file.GetStringArray(sectionname, "FireSound", FireSound);
    }
  }
}
