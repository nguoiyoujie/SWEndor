using MTV3D65;
using SWEndor.Input.Functions;
using System.IO;
using System.Windows.Forms;

namespace SWEndor
{
  public enum ResolutionSettings
  {
    FillScreen, R_640x480, R_800x600, R_1024x768, R_1152x864, R_1280x800, R_1360x768, R_1280x1024, R_1600x900, R_1600x1200, R_1920x1080
  }

  public static class Settings
  {
    public static ResolutionSettings ResolutionMode = ResolutionSettings.FillScreen;
    public static int ResolutionX { get; private set; }
    public static int ResolutionY { get; private set; }
    public static bool GameDebug = false;
    public static bool FullScreenMode = false;
    public static bool ShowPerformance = false;
    public static float SteeringSensitivity = 1.5f;

    public static TV_2DVECTOR GetResolution
    {
      get
      {
        switch (ResolutionMode)
        {
          case ResolutionSettings.R_640x480:
            return new TV_2DVECTOR(640, 480);

          case ResolutionSettings.R_800x600:
            return new TV_2DVECTOR(800, 600);

          case ResolutionSettings.R_1024x768:
            return new TV_2DVECTOR(1024, 768);

          case ResolutionSettings.R_1152x864:
            return new TV_2DVECTOR(1152, 864);

          case ResolutionSettings.R_1280x800:
            return new TV_2DVECTOR(1280, 800);

          case ResolutionSettings.R_1280x1024:
            return new TV_2DVECTOR(1280, 1024);

          case ResolutionSettings.R_1360x768:
            return new TV_2DVECTOR(1360, 768);

          case ResolutionSettings.R_1600x900:
            return new TV_2DVECTOR(1600, 900);

          case ResolutionSettings.R_1600x1200:
            return new TV_2DVECTOR(1600, 1200);

          case ResolutionSettings.R_1920x1080:
            return new TV_2DVECTOR(1920, 1080);

          case ResolutionSettings.FillScreen:
          default:
            return new TV_2DVECTOR(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
        }
      }
    }

    public static void LoadSettings(Engine engine)
    {
      if (File.Exists(Path.Combine(Globals.SettingsPath, "settings.ini")))
      {
        using (StreamReader sr = new StreamReader(Path.Combine(Globals.SettingsPath, "settings.ini")))
        {
          while (!sr.EndOfStream)
          {
            ProcessLine(engine, sr.ReadLine());
          }
        }
      }

      ResolutionX = (int)GetResolution.x;
      ResolutionY = (int)GetResolution.y;
    }

    public static void SaveSettings(Engine engine)
    {
      using (StreamWriter sr = new StreamWriter(Path.Combine(Globals.SettingsPath, "settings.ini"), false))
      {
        sr.WriteLine(string.Format("ResolutionMode={0}", (int)ResolutionMode));
        sr.WriteLine(string.Format("FullScreen={0}", FullScreenMode));
        sr.WriteLine(string.Format("ShowPerformance={0}", ShowPerformance));
        sr.WriteLine(string.Format("GameDebug={0}", GameDebug));
        sr.WriteLine(string.Format("MasterMusicVolume={0}", engine.SoundManager.MasterMusicVolume));
        sr.WriteLine(string.Format("MasterSFXVolume={0}", engine.SoundManager.MasterSFXVolume));
        sr.WriteLine(string.Format("SteeringSensitivity={0}", SteeringSensitivity));

        foreach (InputFunction fn in InputFunction.Registry.GetList())
          if (fn.Name != null && fn.Name.Length > 0)
           sr.WriteLine(string.Format("FuncKey:{0}={1}", fn.Name, fn.Key));
        sr.Flush();
      }
    }

    private static void ProcessLine(Engine engine, string line)
    {
      int seperator_pos = line.IndexOf('=');
      if (seperator_pos != -1 && seperator_pos < line.Length)
      {
        string key = line.Substring(0, seperator_pos).Trim().ToLower();
        string value = line.Substring(seperator_pos + 1).Trim();

        switch (key)
        {
          case "resolutionmode":
            int resM = 0;
            ResolutionMode = (ResolutionSettings)((int.TryParse(value, out resM)) ? resM : 0);
            break;
          case "gamedebug":
            bool gamedebug = false;
            GameDebug = (bool.TryParse(value, out gamedebug)) ? gamedebug : false;
            break;
          case "fullscreen":
            bool fullscr = false;
            FullScreenMode = (bool.TryParse(value, out fullscr)) ? fullscr : false;
            break;
          case "showperformance":
            bool showperf = false;
            ShowPerformance = (bool.TryParse(value, out showperf)) ? showperf : false;
            break;
          case "mastermusicvolume":
            float mastermusicvol = 1.0f;
            engine.SoundManager.MasterMusicVolume = (float.TryParse(value, out mastermusicvol)) ? mastermusicvol : 1.0f;
            break;
          case "mastersfxvolume":
            float mastersfxvol = 1.0f;
            engine.SoundManager.MasterSFXVolume = (float.TryParse(value, out mastersfxvol)) ? mastersfxvol : 1.0f;
            break;
          case "steeringsensitivity":
            float steer = 1.5f;
            SteeringSensitivity = (float.TryParse(value, out steer)) ? steer : 1.5f;
            break;
          default:
            if (key.StartsWith("funckey:") && key.Length > 8)
            {
              string keytype = key.Substring(8); // funckey:
              int funckey = 0;
              funckey = (int.TryParse(value, out funckey)) ? funckey : 0;

              InputFunction fn = InputFunction.Registry.Get(keytype);
              if (fn != null)
                fn.Key = funckey;
            }

            break;
        }
      }
    }
  }
}
