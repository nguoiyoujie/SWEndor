using SWEndor.FileFormat.INI;
using Primrose.Primitives;
using Primrose.Primitives.Extensions;
using System.IO;
using System.Threading.Tasks;
using System;
using SWEndor.Core;
using Primrose.Primitives.Factories;

namespace SWEndor.Weapons
{
  public class WeaponFactory
  {
    private Registry<WeaponStatInfo> list = new Registry<WeaponStatInfo>();
    internal Engine Engine;

    public WeaponFactory(Engine engine)
    {
      Engine = engine;
    }

    public void Register(WeaponStatInfo weapon)
    {
      list.Put(weapon.Name, weapon);
    }

    public WeaponInfo Get(string key)
    {
      WeaponStatInfo wsi = list.Get(key);
      if (wsi == null)
        throw new Exception(TextLocalization.Get(TextLocalKeys.WEAPON_NOTFOUND_ERROR).F(key));
      return new WeaponInfo(Engine, wsi);
    }

    public void LoadFromINI(string filepath)
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
