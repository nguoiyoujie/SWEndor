using SWEndor.FileFormat.INI;

namespace SWEndor.ActorTypes.Components
{
  internal struct DeathCameraData
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
      float radius = f.GetFloat(sectionname, "Radius", Radius);
      float height = f.GetFloat(sectionname, "Height", Height);
      float period = f.GetFloat(sectionname, "Period", Period);
      this = new DeathCameraData(radius, height, period);
    }

    public void SaveToINI(INIFile f, string sectionname)
    {
      f.SetFloat(sectionname, "Radius", Radius);
      f.SetFloat(sectionname, "Height", Height);
      f.SetFloat(sectionname, "Period", Period);
    }
  }
}
