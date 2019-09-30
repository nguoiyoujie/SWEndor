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

    public static void LoadFromINI(INIFile f, string sectionname, string key, out LookData[] dest)
    {
      string[] src = f.GetStringList(sectionname, key, new string[0]);
      dest = new LookData[src.Length];
      for (int i = 0; i < src.Length; i++)
        dest[i].LoadFromINI(f, src[i]);
    }

    public static void SaveToINI(INIFile f, string sectionname, string key, string membername, LookData[] src)
    {
      string[] ss = new string[src.Length];
      for (int i = 0; i < src.Length; i++)
      {
        string s = membername + i.ToString();
        ss[i] = s;
        src[i].SaveToINI(f, s);
      }
      f.SetStringList(sectionname, key, ss);
    }

    private void LoadFromINI(INIFile f, string sectionname)
    {
      LookFrom = f.GetTV_3DVECTOR(sectionname, "LookFrom", LookFrom);
      LookAt = f.GetTV_3DVECTOR(sectionname, "LookAt", LookAt);
    }

    private void SaveToINI(INIFile f, string sectionname)
    {
      f.SetTV_3DVECTOR(sectionname, "LookFrom", LookFrom);
      f.SetTV_3DVECTOR(sectionname, "LookAt", LookAt);
    }
  }
}
