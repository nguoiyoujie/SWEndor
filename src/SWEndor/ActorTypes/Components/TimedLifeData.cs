using SWEndor.FileFormat.INI;

namespace SWEndor.ActorTypes.Components
{
  public struct TimedLifeData
  {
    public readonly bool OnTimedLife;
    public readonly float TimedLife;

    public TimedLifeData(bool enabled, float time)
    {
      OnTimedLife = enabled;
      TimedLife = time;
    }

    public void LoadFromINI(INIFile f, string sectionname)
    {
      float time = f.GetFloatValue(sectionname, "TimedLife", TimedLife);
      bool enable = f.GetBoolValue(sectionname, "OnTimedLife", OnTimedLife);
      this = new TimedLifeData(enable, time);
    }

    public void SaveToINI(INIFile f, string sectionname)
    {
      f.SetFloatValue(sectionname, "TimedLife", TimedLife);
      f.SetBoolValue(sectionname, "OnTimedLife", OnTimedLife);
    }
  }
}
