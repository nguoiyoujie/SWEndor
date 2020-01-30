using Primitives.FileFormat.INI;

namespace SWEndor.Weapons
{
  internal struct WeapAimInfo
  {
    // Auto Aim Bot
    public bool EnablePlayerAutoAim;
    public bool EnableAIAutoAim;
    public float AutoAimMinDeviation;
    public float AutoAimMaxDeviation;

    public static WeapAimInfo Default = new WeapAimInfo
    {
      EnablePlayerAutoAim = false,
      EnableAIAutoAim = false,
      AutoAimMinDeviation = 1,
      AutoAimMaxDeviation = 1
    };

    public void LoadFromINI(INIFile f, string sectionname)
    {
      this = Default;
      EnablePlayerAutoAim = f.GetBool(sectionname, "EnablePlayerAutoAim", EnablePlayerAutoAim);
      EnableAIAutoAim = f.GetBool(sectionname, "EnableAIAutoAim", EnableAIAutoAim);
      AutoAimMinDeviation = f.GetFloat(sectionname, "AutoAimMinDeviation", AutoAimMinDeviation);
      AutoAimMaxDeviation = f.GetFloat(sectionname, "AutoAimMaxDeviation", AutoAimMaxDeviation);
    }
  }
}
