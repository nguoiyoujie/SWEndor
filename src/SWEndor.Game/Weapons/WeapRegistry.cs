using Primrose.Primitives.Factories;
using SWEndor.Game.ActorTypes.Components;
using SWEndor.Game.Core;
using Primrose.FileFormat.INI;
using System.IO;
using System.Threading.Tasks;
using Primrose;

namespace SWEndor.Game.Weapons
{
  internal class WeapRegistry
  {
    private Registry<WeapAimInfo> _aim = new Registry<WeapAimInfo>(256);
    private Registry<WeapAmmoInfo> _ammo = new Registry<WeapAmmoInfo>(256);
    private Registry<WeapLoadInfo> _load = new Registry<WeapLoadInfo>(256);
    private Registry<WeapPortInfo> _port = new Registry<WeapPortInfo>(256);
    private Registry<WeapProjInfo> _proj = new Registry<WeapProjInfo>(256);
    private Registry<WeapTgtInfo> _tgt = new Registry<WeapTgtInfo>(256);

    public void Register(INIFile f, string key, out WeapAimInfo info)
    {
      Log.Info(Globals.LogChannel, LogDecorator.GetFormat(LogType.ASSET_LOADING), "Weapon.Aim", key);
      info = WeapAimInfo.Default;
      f.LoadByAttribute(ref info, key);
      _aim.Add(key, info);
    }

    public void Register(INIFile f, string key, out WeapAmmoInfo info)
    {
      Log.Info(Globals.LogChannel, LogDecorator.GetFormat(LogType.ASSET_LOADING), "Weapon.Ammo", key);
      info = WeapAmmoInfo.Default;
      f.LoadByAttribute(ref info, key);
      _ammo.Add(key, info);
    }

    public void Register(INIFile f, string key, out WeapLoadInfo info)
    {
      Log.Info(Globals.LogChannel, LogDecorator.GetFormat(LogType.ASSET_LOADING), "Weapon.Loader", key);
      info = WeapLoadInfo.Default;
      f.LoadByAttribute(ref info, key);
      _load.Add(key, info);
    }

    public void Register(INIFile f, string key, out WeapPortInfo info)
    {
      Log.Info(Globals.LogChannel, LogDecorator.GetFormat(LogType.ASSET_LOADING), "Weapon.Port", key);
      info = WeapPortInfo.Default;
      f.LoadByAttribute(ref info, key);
      info.Init();
      _port.Add(key, info);
    }

    public void Register(Engine e, INIFile f, string key, out WeapProjInfo info)
    {
      Log.Info(Globals.LogChannel, LogDecorator.GetFormat(LogType.ASSET_LOADING), "Weapon.Projectile", key);
      info = WeapProjInfo.Default;
      f.LoadByAttribute(ref info, key);
      info.Load(e);
      _proj.Add(key, info);
    }

    public void Register(INIFile f, string key, out WeapTgtInfo info)
    {
      Log.Info(Globals.LogChannel, LogDecorator.GetFormat(LogType.ASSET_LOADING), "Weapon.Target", key);
      info = WeapTgtInfo.Default;
      f.LoadByAttribute(ref info, key);
      _tgt.Add(key, info);
    }

    public void Get(string key, out WeapAimInfo info) { info = _aim.Get(key); }
    public void Get(string key, out WeapAmmoInfo info) { info = _ammo.Get(key); }
    public void Get(string key, out WeapLoadInfo info) { info = _load.Get(key); }
    public void Get(string key, out WeapPortInfo info) { info = _port.Get(key); }
    public void Get(string key, out WeapProjInfo info) { info = _proj.Get(key); }
    public void Get(string key, out WeapTgtInfo info) { info = _tgt.Get(key); }

    public void LoadFromINI(Engine e)
    {
      string filepath = Globals.WeapAimINIPath;
      if (File.Exists(filepath))
      {
        INIFile f = new INIFile(filepath);
        Parallel.ForEach(f.Sections,
          (s) =>
          {
            WeapAimInfo w;
            if (s != INIFile.PreHeaderSectionName)
              Register(f, s, out w);
          });
      }

      filepath = Globals.WeapAmmoINIPath;
      if (File.Exists(filepath))
      {
        INIFile f = new INIFile(filepath);
        Parallel.ForEach(f.Sections,
          (s) =>
          {
            WeapAmmoInfo w;
            if (s != INIFile.PreHeaderSectionName)
              Register(f, s, out w);
          });
      }

      filepath = Globals.WeapLoadINIPath;
      if (File.Exists(filepath))
      {
        INIFile f = new INIFile(filepath);
        Parallel.ForEach(f.Sections,
          (s) =>
          {
            WeapLoadInfo w;
            if (s != INIFile.PreHeaderSectionName)
              Register(f, s, out w);
          });
      }

      filepath = Globals.WeapPortINIPath;
      if (File.Exists(filepath))
      {
        INIFile f = new INIFile(filepath);
        Parallel.ForEach(f.Sections,
          (s) =>
          {
            WeapPortInfo w;
            if (s != INIFile.PreHeaderSectionName)
              Register(f, s, out w);
          });
      }

      filepath = Globals.WeapProjINIPath;
      if (File.Exists(filepath))
      {
        INIFile f = new INIFile(filepath);
        Parallel.ForEach(f.Sections,
          (s) =>
          {
            WeapProjInfo w;
            if (s != INIFile.PreHeaderSectionName)
              Register(e, f, s, out w);
          });
      }

      filepath = Globals.WeapTgtINIPath;
      if (File.Exists(filepath))
      {
        INIFile f = new INIFile(filepath);
        Parallel.ForEach(f.Sections,
          (s) =>
          {
            WeapTgtInfo w;
            if (s != INIFile.PreHeaderSectionName)
              Register(f, s, out w);
          });
      }
    }

    public WeaponInfo BuildWeapon(WeapData data)
    {
      return BuildWeapon(data.Name, data.Aim, data.Ammo, data.Port, data.Proj, data.Tgt);
    }

    public WeaponInfo BuildWeapon(string name, string aim, string ammo, string port, string proj, string tgt)
    {
      WeaponInfo ret = new WeaponInfo(name);
      Get(aim, out ret.Aim);
      Get(ammo, out ret.Ammo);
      Get(port, out ret.Port);
      Get(proj, out ret.Proj);
      Get(tgt, out ret.Targeter);

      return ret;
    }
  }
}
