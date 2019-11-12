using SWEndor.FileFormat.INI;

namespace SWEndor.Models
{
  internal struct ExplodeData
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
      string[] src = f.GetStringArray(sectionname, key, new string[0]);
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
      f.SetStringArray(sectionname, key, ss);
    }

    private void LoadFromINI(INIFile f, string sectionname)
    {
      Type = f.GetString(sectionname, "Type", Type);
      Rate = f.GetFloat(sectionname, "Rate", Rate);
      Size = f.GetFloat(sectionname, "Size", Size);
      Trigger = f.GetEnumValue(sectionname, "Trigger", Trigger);
    }

    private void SaveToINI(INIFile f, string sectionname)
    {
      f.SetString(sectionname, "Type", Type);
      f.SetFloat(sectionname, "Rate", Rate);
      f.SetFloat(sectionname, "Size", Size);
      f.SetEnum(sectionname, "Trigger", Trigger);
    }
  }
}