using SWEndor.FileFormat.INI;

namespace SWEndor.ActorTypes.Components
{
  internal struct TimedLifeData
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
      float time = f.GetFloat(sectionname, "TimedLife", TimedLife);
      bool enable = f.GetBool(sectionname, "OnTimedLife", OnTimedLife);
      this = new TimedLifeData(enable, time);
    }

    public void SaveToINI(INIFile f, string sectionname)
    {
      f.SetFloat(sectionname, "TimedLife", TimedLife);
      f.SetBool(sectionname, "OnTimedLife", OnTimedLife);
    }
  }
}
