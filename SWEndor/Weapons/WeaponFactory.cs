using SWEndor.FileFormat.INI;
using SWEndor.Primitives;
using SWEndor.Primitives.Extensions;
using System.IO;
using System.Threading.Tasks;

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
        throw new System.Exception(TextLocalization.Get(TextLocalKeys.WEAPON_NOTFOUND_ERROR).F(key));
      return new WeaponInfo(Globals.Engine, wsi);
    }

    public static void LoadFromINI(string filepath)
    {
      if (File.Exists(filepath))
      {
        INIFile f = new INIFile(filepath);
        string main = "Weapons";
        if (f.HasSection(main))
        {
          INIFile.INISection weaps = f.GetSection(main);
          Parallel.ForEach(weaps.GetKeys(),
            (s) =>
            {
              if (s != INIFile.PreHeaderSectionName)
                Register(new WeaponStatInfo(f, s));
            });
        }
      }
    }
  }
}
