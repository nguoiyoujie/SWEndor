using SWEndor.Game.Actors.Components;
using SWEndor.Game.Actors.Models;
using Primrose.FileFormat.INI;
using SWEndor.Game.Weapons;
using System.Collections.Generic;

namespace SWEndor.Game.ActorTypes.Components
{
  internal struct SystemData
  {
    //static SystemData()
    //{
    //  Array a = Enum.GetValues(typeof(SystemPart));
    //  AllParts = new SystemPart[a.Length];
    //  for (int i = 0; i < a.Length; i++)
    //    AllParts[i] = (SystemPart)a.GetValue(i);
    //}

    [INIValue]
    public float MaxShield;

    [INIValue]
    public float MaxHull;

    [INIValue]
    public float Energy_Income;

    [INIValue]
    public float Energy_NoChargerIncome;

    [INIValue]
    public float MaxEnergy_inStore;

    [INIValue]
    public float MaxEnergy_inEngine;

    [INIValue]
    public float MaxEnergy_inLasers;

    [INIValue]
    public float MaxEnergy_inShields;

    [INIValue]
    public float Energy_TransferRate;

    [INIValue]
    public bool AllowSystemDamage;

    [INISubSectionList(SubsectionPrefix = "PART")]
    internal SystemInstrumentData[] Parts;

    //[INIValue]
    //public float CriticalFailureChance;

    //[INIValue]
    //public float RecoveryTime;


    //private static readonly SystemPart[] AllParts;
    //private static readonly SystemPart[] NoParts = new SystemPart[0];
    public void Reset()
    {
      //Parts = NoParts;
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
      //Parts = AllParts;
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
        hasLaser |= (wi.Proj.WeaponType == WeaponType.LASER || wi.Proj.WeaponType == WeaponType.ION);
        hasProj |= (wi.Proj.WeaponType == WeaponType.MISSILE || wi.Proj.WeaponType == WeaponType.TORPEDO);
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

      //Parts = parts.ToArray();
    }
  }
}
