using MTV3D65;
using SWEndor.ActorTypes;
using SWEndor.FileFormat.INI;
using System;

namespace SWEndor.Weapons
{
  public class WeaponStatInfo
  {
    public WeaponStatInfo(string name)
    {
      Name = name;
    }

    public readonly string Name = "Null Weapon";

    public string WeaponProjectile = null;
    public float WeaponCooldown = 0;
    public float WeaponCooldownRate = 1;
    public float WeaponCooldownRateRandom = 0;

    public int Burst = 1;
    public int Ammo = -1;
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
    public bool RequirePlayerTargetLock = false;

    // AI Config
    public TargetType AIAttackTargets = TargetType.ANY;

    public bool AIAttackNull = true;

    public float AngularRange = 10;
    public float Range = 4500;

    public string FireSound = "laser_sf";

    public WeaponStatInfo(INIFile file, string sectionname)
    {
      Name = sectionname;

      WeaponProjectile = file.GetStringValue(sectionname, "WeaponProjectile", WeaponProjectile);
      WeaponCooldown = file.GetFloatValue(sectionname, "WeaponCooldown", WeaponCooldown);
      WeaponCooldownRate = file.GetFloatValue(sectionname, "WeaponCooldownRate", WeaponCooldownRate);
      WeaponCooldownRateRandom = file.GetFloatValue(sectionname, "WeaponCooldownRateRandom", WeaponCooldownRateRandom);

      Burst = file.GetIntValue(sectionname, "Burst", Burst);
      Ammo = file.GetIntValue(sectionname, "Ammo", Ammo);
      MaxAmmo = file.GetIntValue(sectionname, "MaxAmmo", MaxAmmo);
      AmmoReloadCooldown = file.GetFloatValue(sectionname, "AmmoReloadCooldown", AmmoReloadCooldown);
      AmmoReloadRate = file.GetFloatValue(sectionname, "AmmoReloadRate", AmmoReloadRate);
      AmmoReloadRateRandom = file.GetFloatValue(sectionname, "AmmoReloadRateRandom", AmmoReloadRateRandom);
      AmmoReloadAmount = file.GetIntValue(sectionname, "AmmoReloadAmount", AmmoReloadAmount);
      ProjectileWaitBeforeHoming = file.GetFloatValue(sectionname, "ProjectileWaitBeforeHoming", ProjectileWaitBeforeHoming);

      float[] fpos = file.GetFloatList(sectionname, "FirePositions", new float[0]);
      FirePositions = new TV_3DVECTOR[fpos.Length / 3];
      for (int p = 0; p + 2 < fpos.Length; p += 3)
        FirePositions[p / 3] = new TV_3DVECTOR(fpos[p], fpos[p + 1], fpos[p + 2]);

      CurrentPositionIndex = file.GetIntValue(sectionname, "CurrentPositionIndex", CurrentPositionIndex);

      // Auto Aim Bot
      EnablePlayerAutoAim = file.GetBoolValue(sectionname, "EnablePlayerAutoAim", EnablePlayerAutoAim);
      EnableAIAutoAim = file.GetBoolValue(sectionname, "EnableAIAutoAim", EnableAIAutoAim);
      AutoAimMinDeviation = file.GetFloatValue(sectionname, "AutoAimMinDeviation", AutoAimMinDeviation);
      AutoAimMaxDeviation = file.GetFloatValue(sectionname, "AutoAimMaxDeviation", AutoAimMaxDeviation);

      // Player Config
      RequirePlayerTargetLock = file.GetBoolValue(sectionname, "RequirePlayerTargetLock", RequirePlayerTargetLock);

      // AI Config
      string[] tgts = file.GetStringValue(sectionname, "AIAttackTargets", AIAttackTargets.ToString()).Split('|', ',');
      TargetType tgtt = TargetType.NULL;
      foreach (string t in tgts)
        tgtt |= (TargetType)Enum.Parse(typeof(TargetType), t.Trim());

      AIAttackTargets = tgtt;
      AIAttackNull = file.GetBoolValue(sectionname, "AIAttackNull", AIAttackNull);

      AngularRange = file.GetFloatValue(sectionname, "AngularRange", AngularRange);
      Range = file.GetFloatValue(sectionname, "Range", Range);

      FireSound = file.GetStringValue(sectionname, "FireSound", FireSound);
    }

    /*
  public static class WeaponParser
  {
    public static WeaponInfo Parse(INIFile file, string sectionname)
    {
      WeaponInfo w = new WeaponInfo(sectionname);

      string proj = file.GetStringValue(sectionname, "WeaponProjectile", null);
      w.WeaponProjectile = (proj != null) ? ActorTypeInfo.Factory.Get(proj) as ProjectileGroup : null;
      w.WeaponCooldown = file.GetFloatValue(sectionname, "WeaponCooldown", w.WeaponCooldown);
      w.WeaponCooldownRate = file.GetFloatValue(sectionname, "WeaponCooldownRate", w.WeaponCooldownRate);
      w.WeaponCooldownRateRandom = file.GetFloatValue(sectionname, "WeaponCooldownRateRandom", w.WeaponCooldownRateRandom);

      w.Burst = file.GetIntValue(sectionname, "Burst", w.Burst);
      w.Ammo = file.GetIntValue(sectionname, "Ammo", w.Ammo);
      w.MaxAmmo = file.GetIntValue(sectionname, "MaxAmmo", w.MaxAmmo);
      w.AmmoReloadCooldown = file.GetFloatValue(sectionname, "AmmoReloadCooldown", w.AmmoReloadCooldown);
      w.AmmoReloadRate = file.GetFloatValue(sectionname, "AmmoReloadRate", w.AmmoReloadRate);
      w.AmmoReloadRateRandom = file.GetFloatValue(sectionname, "AmmoReloadRateRandom", w.AmmoReloadRateRandom);
      w.AmmoReloadAmount = file.GetIntValue(sectionname, "AmmoReloadAmount", w.AmmoReloadAmount);
      w.ProjectileWaitBeforeHoming = file.GetFloatValue(sectionname, "ProjectileWaitBeforeHoming", w.ProjectileWaitBeforeHoming);

      float[] fpos = file.GetFloatList(sectionname, "FirePositions", new float[0]);
      w.FirePositions = new TV_3DVECTOR[fpos.Length / 3];
      for (int p = 0; p + 2 < fpos.Length; p += 3)
        w.FirePositions[p / 3] = new TV_3DVECTOR(fpos[p], fpos[p + 1], fpos[p + 2]);

      w.CurrentPositionIndex = file.GetIntValue(sectionname, "CurrentPositionIndex", w.CurrentPositionIndex);

      // Auto Aim Bot
      w.EnablePlayerAutoAim = file.GetBoolValue(sectionname, "EnablePlayerAutoAim", w.EnablePlayerAutoAim);
      w.EnableAIAutoAim = file.GetBoolValue(sectionname, "EnableAIAutoAim", w.EnableAIAutoAim);
      w.AutoAimMinDeviation = file.GetFloatValue(sectionname, "AutoAimMinDeviation", w.AutoAimMinDeviation);
      w.AutoAimMaxDeviation = file.GetFloatValue(sectionname, "AutoAimMaxDeviation", w.AutoAimMaxDeviation);

      // Player Config
      w.RequirePlayerTargetLock = file.GetBoolValue(sectionname, "RequirePlayerTargetLock", w.RequirePlayerTargetLock);

      // AI Config
      string[] tgts = file.GetStringValue(sectionname, "AIAttackTargets", w.AIAttackTargets.ToString()).Split('|', ',');
      TargetType tgtt = TargetType.NULL;
      foreach (string t in tgts)
        tgtt |= (TargetType)Enum.Parse(typeof(TargetType), t.Trim());

      w.AIAttackTargets = tgtt;
      w.AIAttackNull = file.GetBoolValue(sectionname, "AIAttackNull", w.AIAttackNull);

      w.AngularRange = file.GetFloatValue(sectionname, "AngularRange", w.AngularRange);
      w.Range = file.GetFloatValue(sectionname, "Range", w.Range);

      w.FireSound = file.GetStringValue(sectionname, "FireSound", w.FireSound);

      return w;
    }
  */
  }
}
