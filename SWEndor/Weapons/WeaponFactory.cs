using SWEndor.FileFormat.INI;
using SWEndor.Weapons.Types;
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
        return new DummyWeapon();
      return new WeaponInfo(wsi);
    }

    public static void LoadFromINI(string filepath)
    {
      if (File.Exists(filepath))
      {
        INIFile f = new INIFile(filepath);
        foreach (string s in f.Sections.Keys)
        {
          if (s != INIFile.PreHeaderSectionName)
            Register(new WeaponStatInfo(f, s));//WeaponParser.Parse(f, s));
        }
      }
    }
  }
}
