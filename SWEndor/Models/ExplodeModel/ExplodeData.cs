using SWEndor.ExplosionTypes;
using SWEndor.FileFormat.INI;

namespace SWEndor.Models
{
  public struct ExplodeData
  {
    public string Type;
    public float Rate;
    public float Size;
    public ExplodeTrigger Trigger;

    public ExplodeData(string type, float rate, float size, ExplodeTrigger trigger)
    {
      Type = type;
      Rate = rate;
      Size = size;
      Trigger = trigger;
    }

    public static void LoadFromINI(INIFile f, string sectionname, string key, out ExplodeData[] dest)
    {
      string[] src = f.GetStringList(sectionname, key, new string[0]);
      dest = new ExplodeData[src.Length];
      for (int i = 0; i < src.Length; i++)
        dest[i].LoadFromINI(f, src[i]);
    }

    public static void SaveToINI(INIFile f, string sectionname, string key, string membername, ExplodeData[] src)
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
      Type = f.GetStringValue(sectionname, "Type", Type);
      Rate = f.GetFloatValue(sectionname, "Rate", Rate);
      Size = f.GetFloatValue(sectionname, "Size", Size);
      Trigger = f.GetEnumValue<ExplodeTrigger>(sectionname, "Trigger", Trigger);
    }

    private void SaveToINI(INIFile f, string sectionname)
    {
      f.SetStringValue(sectionname, "Type", Type);
      f.SetFloatValue(sectionname, "Rate", Rate);
      f.SetFloatValue(sectionname, "Size", Size);
      f.SetEnumValue<ExplodeTrigger>(sectionname, "Trigger", Trigger);
    }
  }
}