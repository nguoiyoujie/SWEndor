using SWEndor.FileFormat.INI;
using SWEndor.Primitives;
using System.IO;

namespace SWEndor.Weapons
{
  public static class WeaponFactory
  {
    private static ThreadSafeDictionary<string, WeaponStatInfo> list = new ThreadSafeDictionary<string, WeaponStatInfo>();

    public static void Register(WeaponStatInfo weapon)
    {
      list.Put(weapon.Name, weapon);
    }

    public static WeaponInfo Get(string key)
    {
      WeaponStatInfo wsi = list.Get(key);
      if (wsi == null)
        throw new System.Exception("Weapon '" + key + "' is not found!");
      return new WeaponInfo(wsi);
    }

    public static void LoadFromINI(string filepath)
    {
      if (File.Exists(filepath))
      {
        INIFile f = new INIFile(filepath);

        // [Weapons]
        // X=weapon1
        //
        // [weapon1]
        // ...

        string main = "Weapons";
        if (f.HasSection(main))
        {
          INIFile.INISection weaps = f.GetSection(main);
          foreach (string s in weaps.GetKeys())
          {
            if (s != INIFile.PreHeaderSectionName)
              Register(new WeaponStatInfo(f, s));
          }
        }
      }
    }
  }
}
