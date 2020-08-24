using Primrose.Primitives.ValueTypes;
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
    public float2 ReloadRate; // rate, random

    [INIValue]
    public int ReloadAmount;

    private float ReloadCooldown;

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
  }
}
