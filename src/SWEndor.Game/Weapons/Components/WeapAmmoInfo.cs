using SWEndor.Game.Core;
using Primrose.FileFormat.INI;

namespace SWEndor.Game.Weapons
{
  internal struct WeapAmmoInfo
  {
    [INIValue("Max")] // initialize Count = Max
    public int Count;

    [INIValue]
    public int Max;

    [INIValue]
    public float ReloadRate;

    [INIValue]
    public float ReloadRateRandom;

    [INIValue]
    public int ReloadAmount;

    private float ReloadCooldown;

    public static WeapAmmoInfo Default = new WeapAmmoInfo
    {
      Count = -1,
      Max = -1,
      ReloadRate = 1,
      ReloadRateRandom = 0,
      ReloadAmount = 1
    };

    public void Reload(Engine engine)
    {
      if (Count == Max)
        ReloadCooldown = engine.Game.GameTime + ReloadRate;
      else if (Max > 0 && ReloadCooldown < engine.Game.GameTime && Count < Max)
      {
        ReloadCooldown = engine.Game.GameTime + ReloadRate;
        if (ReloadRateRandom != 0)
          ReloadCooldown += (float)engine.Random.NextDouble() * ReloadRateRandom;

        Count += ReloadAmount;
        if (Count > Max)
          Count = Max;
      }
    }
  }
}
