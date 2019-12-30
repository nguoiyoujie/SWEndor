using SWEndor.Actors.Components;
using SWEndor.Actors.Models;
using SWEndor.FileFormat.INI;
using SWEndor.Weapons;
using System;
using System.Collections.Generic;

namespace SWEndor.ActorTypes.Components
{
  internal struct SystemData
  {
    static SystemData()
    {
      Array a = Enum.GetValues(typeof(SystemPart));
      AllParts = new SystemPart[a.Length];
      for (int i = 0; i < a.Length; i++)
        AllParts[i] = (SystemPart)a.GetValue(i);
    }

    public SystemPart[] Parts;

    public float MaxShield;
    public float MaxHull;
    public float Energy_Income;
    public float Energy_NoChargerIncome;
    public float MaxEnergy_inStore;
    public float MaxEnergy_inEngine;
    public float MaxEnergy_inLasers;
    public float MaxEnergy_inShields;
    public float Energy_TransferRate;
    public bool AllowSystemDamage;

    private static SystemPart[] AllParts;
    private static SystemPart[] NoParts = new SystemPart[0];
    public void Reset()
    {
      Parts = NoParts;
      MaxShield = 0;
      MaxHull = 1;
      Energy_Income = 0;
      Energy_NoChargerIncome = 0;
      MaxEnergy_inStore = 1;
      MaxEnergy_inEngine = 1;
      MaxEnergy_inLasers = 1;
      MaxEnergy_inShields = 1;
      Energy_TransferRate = 1;
      AllowSystemDamage = false;
    }

    public void GetAllParts()
    {
      Parts = AllParts;
    }

    /// <summary>
    /// Automatically generate parts. Use after other systems have been initialized.
    /// </summary>
    /// <param name="atype"></param>
    public void AutoParts(ActorTypeInfo atype)
    {
      List<SystemPart> parts = new List<SystemPart>(16);

      if (atype.MoveLimitData.MaxSpeed > 0)
        parts.Add(SystemPart.ENGINE);

      if (atype.MoveLimitData.MaxTurnRate > 0)
        parts.Add(SystemPart.SIDE_THRUSTERS);

      if (Energy_Income > 0 && Energy_Income > Energy_NoChargerIncome)
        parts.Add(SystemPart.ENERGY_CHARGER);

      if (MaxEnergy_inStore > 0)
        parts.Add(SystemPart.ENERGY_STORE);

      WeaponData w = atype.cachedWeaponData.Fix(atype.Engine.WeaponRegistry);
      bool hasLaser = false;
      bool hasProj = false;
      foreach (WeaponInfo wi in w.Weapons)
      {
        hasLaser |= (wi.Proj.Type == WeaponType.LASER || wi.Proj.Type == WeaponType.ION);
        hasProj |= (wi.Proj.Type == WeaponType.MISSILE || wi.Proj.Type == WeaponType.TORPEDO);
      }

      if (hasLaser)
        parts.Add(SystemPart.LASER_WEAPONS);

      if (hasProj)
        parts.Add(SystemPart.PROJECTILE_LAUNCHERS);

      if (MaxShield > 0)
        parts.Add(SystemPart.SHIELD_GENERATOR);

      // All ships have this
      parts.Add(SystemPart.RADAR);
      parts.Add(SystemPart.SCANNER);
      parts.Add(SystemPart.TARGETING_SYSTEM);
      parts.Add(SystemPart.COMLINK);

      // TO-DO: Hyperdrive system
      // parts.Add(SystemPart.HYPERDRIVE);

      Parts = parts.ToArray();
    }

    public void LoadFromINI(INIFile f, string sectionname)
    {
      MaxShield = f.GetFloat(sectionname, "MaxShield", MaxShield);
      MaxHull = f.GetFloat(sectionname, "MaxHull", MaxHull);
      Energy_Income = f.GetFloat(sectionname, "Energy_Income", Energy_Income);
      Energy_NoChargerIncome = f.GetFloat(sectionname, "Energy_NoChargerIncome", Energy_NoChargerIncome);
      MaxEnergy_inStore = f.GetFloat(sectionname, "MaxEnergy_inStore", MaxEnergy_inStore);
      MaxEnergy_inEngine = f.GetFloat(sectionname, "MaxEnergy_inEngine", MaxEnergy_inEngine);
      MaxEnergy_inLasers = f.GetFloat(sectionname, "MaxEnergy_inLasers", MaxEnergy_inLasers);
      MaxEnergy_inShields = f.GetFloat(sectionname, "MaxEnergy_inShields", MaxEnergy_inShields);
      Energy_TransferRate = f.GetFloat(sectionname, "Energy_TransferRate", Energy_TransferRate);
      AllowSystemDamage = f.GetBool(sectionname, "AllowSystemDamage", AllowSystemDamage);
      Parts = f.GetEnumArray(sectionname, "Parts", new SystemPart[0]);
    }

    public void SaveToINI(INIFile f, string sectionname)
    {
      f.SetFloat(sectionname, "MaxShield", MaxShield);
      f.SetFloat(sectionname, "MaxHull", MaxHull);
      f.SetFloat(sectionname, "Energy_Income", Energy_Income);
      f.SetFloat(sectionname, "Energy_NoChargerIncome", Energy_NoChargerIncome);
      f.SetFloat(sectionname, "MaxEnergy_inStore", MaxEnergy_inStore);
      f.SetFloat(sectionname, "MaxEnergy_inEngine", MaxEnergy_inEngine);
      f.SetFloat(sectionname, "MaxEnergy_inLasers", MaxEnergy_inLasers);
      f.SetFloat(sectionname, "MaxEnergy_inShields", MaxEnergy_inShields);
      f.SetFloat(sectionname, "Energy_TransferRate", Energy_TransferRate);
      f.SetBool(sectionname, "AllowSystemDamage", AllowSystemDamage);
      f.SetEnumArray(sectionname, "Parts", new SystemPart[0]);
    }
  }
}
