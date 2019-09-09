using MTV3D65;
using SWEndor.FileFormat.INI;

namespace SWEndor.ActorTypes.Components
{
  public struct LookData
  {
    public TV_3DVECTOR LookFrom;
    public TV_3DVECTOR LookAt;

    public LookData(TV_3DVECTOR from, TV_3DVECTOR to)
    {
      LookFrom = from;
      LookAt = to;
    }

    public void LoadFromINI(INIFile f, string sectionname)
    {
      LookFrom = f.GetTV_3DVECTOR(sectionname, "LookFrom", LookFrom);
      LookAt = f.GetTV_3DVECTOR(sectionname, "LookAt", LookAt);
    }

    public void SaveToINI(INIFile f, string sectionname)
    {
      f.SetTV_3DVECTOR(sectionname, "LookFrom", LookFrom);
      f.SetTV_3DVECTOR(sectionname, "LookAt", LookAt);
    }
  }
}
