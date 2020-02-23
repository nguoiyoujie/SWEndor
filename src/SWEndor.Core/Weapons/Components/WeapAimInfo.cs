using Primitives.FileFormat.INI;

namespace SWEndor.Weapons
{
  internal struct WeapAimInfo
  {
    private const string sNone = "";

    // Auto Aim Bot
    [INIValue(sNone, "EnablePlayerAutoAim")]
    public bool EnablePlayerAutoAim;

    [INIValue(sNone, "EnableAIAutoAim")]
    public bool EnableAIAutoAim;

    [INIValue(sNone, "AutoAimMinDeviation")]
    public float AutoAimMinDeviation;

    [INIValue(sNone, "AutoAimMaxDeviation")]
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
