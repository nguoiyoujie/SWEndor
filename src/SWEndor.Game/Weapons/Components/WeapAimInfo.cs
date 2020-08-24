using Primrose.FileFormat.INI;

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
  }
}
