using Primrose.Primitives.ValueTypes;
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
      float3 rhp = f.GetFloat3(sectionname, "DeathCam", new float3(Radius, Height, Period));
      this = new DeathCameraData(rhp.x, rhp.y, rhp.z);
    }

    public void SaveToINI(INIFile f, string sectionname)
    {
      f.SetFloat3(sectionname, "DeathCam", new float3(Radius, Height, Period));
    }
  }
}
