using Primrose.Primitives.ValueTypes;
using SWEndor.Core;
using SWEndor.FileFormat.INI;

namespace SWEndor.Weapons
{
  internal struct WeapAmmoInfo
  {
    public int Count;
    public int Max;
    public float2 ReloadRate; // rate, random
    public int ReloadAmount;

    public float ReloadCooldown;

    public static WeapAmmoInfo Default = new WeapAmmoInfo
    {
      Count = -1,
      Max = -1,
      ReloadRate = new float2(1, 0),
      ReloadAmount = 1
    };

    public void Reload(Engine engine)
    {
      if (Count == Max)
        ReloadCooldown = engine.Game.GameTime + ReloadRate.x;
      else if (Max > 0 && ReloadCooldown < engine.Game.GameTime && Count < Max)
      {
        ReloadCooldown = engine.Game.GameTime + ReloadRate.x;
        if (ReloadRate.y != 0)
          ReloadCooldown += (float)engine.Random.NextDouble() * ReloadRate.y;

        Count += ReloadAmount;
        if (Count > Max)
          Count = Max;
      }
    }

    public void LoadFromINI(INIFile f, string sectionname)
    {
      this = Default;
      //Burst = f.GetInt(sectionname, "Burst", Burst);
      //Count = f.GetInt(sectionname, "Count", Count);
      Max = f.GetInt(sectionname, "Max", Max);
      Count = Max;
      ReloadRate = f.GetFloat2(sectionname, "ReloadRate", ReloadRate);
      ReloadAmount = f.GetInt(sectionname, "ReloadAmount", ReloadAmount);
    }
  }
}
