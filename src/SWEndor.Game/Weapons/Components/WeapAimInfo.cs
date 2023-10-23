using Primrose.FileFormat.INI;
using SWEndor.Game.Core;

namespace SWEndor.Game.Weapons
{
  internal struct WeapAimInfo
  {
    // Auto Aim Bot
    [INIValue]
    public bool EnablePlayerAutoAim;

    [INIValue]
    public bool EnableAIAutoAim;

    [INIValue]
    public float AutoAimMinDeviation;

    [INIValue]
    public float AutoAimMaxDeviation;

    public static WeapAimInfo Default = new WeapAimInfo
    {
      EnablePlayerAutoAim = false,
      EnableAIAutoAim = false,
      AutoAimMinDeviation = 1,
      AutoAimMaxDeviation = 1
    };

    public bool IsAutoAim(bool isPlayer)
    {
      return isPlayer ? EnablePlayerAutoAim : EnableAIAutoAim;
    }

    public float ApplyDeviation(Engine engine, float speed, float distance)
    {
      if (speed == 0)
        return distance;

      if (AutoAimMaxDeviation == AutoAimMinDeviation)
        return distance / speed * AutoAimMinDeviation;

      return distance / speed * (AutoAimMinDeviation + (AutoAimMaxDeviation - AutoAimMinDeviation) * (float)engine.Random.NextDouble());
    }
  }
}
