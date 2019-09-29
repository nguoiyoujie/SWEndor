using SWEndor.FileFormat.INI;

namespace SWEndor.ActorTypes.Components
{
  public struct DeathCameraData
  {
    public readonly float Radius;
    public readonly float Height;
    public readonly float Period;

    public DeathCameraData(float radius, float height, float period)
    {
      Radius = radius;
      Height = height;
      Period = period;
    }

    public void LoadFromINI(INIFile f, string sectionname)
    {
      float radius = f.GetFloatValue(sectionname, "Radius", Radius);
      float height = f.GetFloatValue(sectionname, "Height", Height);
      float period = f.GetFloatValue(sectionname, "Period", Period);
      this = new DeathCameraData(radius, height, period);
    }

    public void SaveToINI(INIFile f, string sectionname)
    {
      f.SetFloatValue(sectionname, "Radius", Radius);
      f.SetFloatValue(sectionname, "Height", Height);
      f.SetFloatValue(sectionname, "Period", Period);
    }
  }
}
