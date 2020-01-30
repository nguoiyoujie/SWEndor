using Primitives.FileFormat.INI;

namespace SWEndor.ActorTypes.Components
{
  internal struct WeapData
  {
    public string Name;
    public string Load;
    public string Aim;
    public string Ammo;
    public string Port;
    public string Proj;
    public string Tgt;

    public WeapData(string name, string load, string aim, string ammo, string port, string proj, string tgt)
    {
      Name = name;
      Load = load;
      Aim = aim;
      Ammo = ammo;
      Port = port;
      Proj = proj;
      Tgt = tgt;
    }

    public static void LoadFromINI(INIFile f, string sectionname, string key, out WeapData[] dest)
    {
      string[] src = f.GetStringArray(sectionname, key, new string[0]);
      dest = new WeapData[src.Length];
      for (int i = 0; i < src.Length; i++)
        dest[i].LoadFromINI(f, src[i]);
    }

    public static void SaveToINI(INIFile f, string sectionname, string key, string membername, WeapData[] src)
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
      string name = f.GetString(sectionname, "Name", Name);
      string load = f.GetString(sectionname, "Load", Load);
      string aim = f.GetString(sectionname, "Aim", Aim);
      string ammo = f.GetString(sectionname, "Ammo", Ammo);
      string port = f.GetString(sectionname, "Port", Port);
      string proj = f.GetString(sectionname, "Proj", Proj);
      string tgt = f.GetString(sectionname, "Tgt", Tgt);

      this = new WeapData(name, load,  aim, ammo, port, proj, tgt);
    }

    private void SaveToINI(INIFile f, string sectionname)
    {
      f.SetString(sectionname, "Name", Name);
      f.SetString(sectionname, "Load", Load);
      f.SetString(sectionname, "Aim", Aim);
      f.SetString(sectionname, "Ammo", Ammo);
      f.SetString(sectionname, "Port", Port);
      f.SetString(sectionname, "Proj", Proj);
      f.SetString(sectionname, "Tgt", Tgt);
    }
  }
}
