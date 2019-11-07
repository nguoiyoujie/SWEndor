using SWEndor.FileFormat.INI;
using SWEndor.Models;

namespace SWEndor.ActorTypes.Components
{
  public struct RenderData
  {
    // AI
    public bool EnableDistanceCull { get { return CullDistance > 0; } }
    public float CullDistance;
    public float RadarSize;
    public RadarType RadarType;
    public bool AlwaysShowInRadar;

    public readonly static RenderData Default =
        new RenderData
        {
          CullDistance = 20000,
        };


    public void LoadFromINI(INIFile f, string sectionname)
    {
      CullDistance = f.GetFloatValue(sectionname, "CullDistance", CullDistance);
      RadarSize = f.GetFloatValue(sectionname, "RadarSize", RadarSize);
      RadarType = f.GetEnumValue(sectionname, "RadarType", RadarType);
      AlwaysShowInRadar = f.GetBoolValue(sectionname, "AlwaysShowInRadar", AlwaysShowInRadar);
    }

    public void SaveToINI(INIFile f, string sectionname)
    {
      f.SetFloatValue(sectionname, "CullDistance", CullDistance);
      f.SetFloatValue(sectionname, "RadarSize", RadarSize);
      f.SetEnumValue(sectionname, "RadarType", RadarType);
      f.SetBoolValue(sectionname, "AlwaysShowInRadar", AlwaysShowInRadar);
    }
  }
}

