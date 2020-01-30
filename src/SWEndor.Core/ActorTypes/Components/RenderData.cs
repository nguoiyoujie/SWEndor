using Primitives.FileFormat.INI;
using SWEndor.Models;

namespace SWEndor.ActorTypes.Components
{
  internal struct RenderData
  {
    public bool EnableDistanceCull { get { return CullDistance > 0; } }
    public float CullDistance;
    public float RadarSize;
    public RadarType RadarType;
    public bool AlwaysShowInRadar;

    public float ZEaseInTime;
    public float ZEaseInDelay;
    public bool RemapLaserColor;

    public readonly static RenderData Default =
        new RenderData
        {
          CullDistance = 20000,
          ZEaseInTime = -1,
        };


    public void LoadFromINI(INIFile f, string sectionname)
    {
      CullDistance = f.GetFloat(sectionname, "CullDistance", CullDistance);
      RadarSize = f.GetFloat(sectionname, "RadarSize", RadarSize);
      RadarType = f.GetEnum(sectionname, "RadarType", RadarType);
      AlwaysShowInRadar = f.GetBool(sectionname, "AlwaysShowInRadar", AlwaysShowInRadar);
      ZEaseInTime = f.GetFloat(sectionname, "ZEaseInTime", ZEaseInTime);
      ZEaseInDelay = f.GetFloat(sectionname, "ZEaseInDelay", ZEaseInDelay);
      RemapLaserColor = f.GetBool(sectionname, "RemapLaserColor", RemapLaserColor);
    }

    public void SaveToINI(INIFile f, string sectionname)
    {
      f.SetFloat(sectionname, "CullDistance", CullDistance);
      f.SetFloat(sectionname, "RadarSize", RadarSize);
      f.SetEnum(sectionname, "RadarType", RadarType);
      f.SetBool(sectionname, "AlwaysShowInRadar", AlwaysShowInRadar);
      f.SetFloat(sectionname, "ZEaseInTime", ZEaseInTime);
      f.SetFloat(sectionname, "ZEaseInDelay", ZEaseInDelay);
      f.SetBool(sectionname, "RemapLaserColor", RemapLaserColor);
    }
  }
}

