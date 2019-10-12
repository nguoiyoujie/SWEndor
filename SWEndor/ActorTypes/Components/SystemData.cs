using SWEndor.Actors.Models;
using System;

namespace SWEndor.ActorTypes.Components
{
  public struct SystemData
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
    }

    public void GetAllParts()
    {
      Parts = AllParts;
    }
  }
}
