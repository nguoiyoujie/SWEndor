using SWEndor.FileFormat.INI;
using SWEndor.Primitives;
using System.IO;

namespace SWEndor.Weapons
{
  public static class WeaponLoadoutFactory
  {
    private static ThreadSafeDictionary<string, WeaponLoadoutInfo> list = new ThreadSafeDictionary<string, WeaponLoadoutInfo>();

    public static void Register(WeaponLoadoutInfo weapon)
    {
      list.Put(weapon.Name, weapon);
    }

    public static WeaponLoadoutInfo Get(string key)
    {
      WeaponLoadoutInfo wsi = list.Get(key);
      if (wsi == null)
        throw new System.Exception("Weapon Loadout '" + key + "' is not found!");
      return wsi;
    }

    public static void LoadFromINI(string filepath)
    {
      if (File.Exists(filepath))
      {
        INIFile f = new INIFile(filepath);

        string main = "WeaponLoadouts";
        if (f.HasSection(main))
        {
          INIFile.INISection weaps = f.GetSection(main);
          foreach (string s in weaps.GetKeys())
          {
            if (s != INIFile.PreHeaderSectionName)
              Register(new WeaponLoadoutInfo(f, s));
          }
        }
      }
    }
  }
}
