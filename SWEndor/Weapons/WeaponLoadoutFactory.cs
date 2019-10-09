﻿using SWEndor.FileFormat.INI;
using SWEndor.Primitives;
using SWEndor.Primitives.Extensions;
using System.IO;
using System.Threading.Tasks;

namespace SWEndor.Weapons
{
  public class WeaponLoadoutFactory
  {
    private ThreadSafeDictionary<string, WeaponLoadoutInfo> list = new ThreadSafeDictionary<string, WeaponLoadoutInfo>();

    public void Register(WeaponLoadoutInfo weapon)
    {
      list.Put(weapon.Name, weapon);
    }

    public WeaponLoadoutInfo Get(string key)
    {
      WeaponLoadoutInfo wsi = list.Get(key);
      if (wsi == null)
        throw new System.Exception(TextLocalization.Get(TextLocalKeys.WEAPONLOAD_NOTFOUND_ERROR).F(key));
      return wsi;
    }

    public void LoadFromINI(string filepath)
    {
      if (File.Exists(filepath))
      {
        INIFile f = new INIFile(filepath);

        string main = "WeaponLoadouts";
        if (f.HasSection(main))
        {
          INIFile.INISection weaps = f.GetSection(main);
          Parallel.ForEach(weaps.GetKeys(),
            (s) =>
            {
              if (s != INIFile.PreHeaderSectionName)
                Register(new WeaponLoadoutInfo(f, s));
            });
        }
      }
    }
  }
}
