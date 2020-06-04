using MTV3D65;
using SWEndor.Core;
using Primitives.FileFormat.INI;
using SWEndor.Input.Functions;
using System.IO;
using System.Windows.Forms;

namespace SWEndor
{
  public enum ResolutionSettings
  {
    FillScreen, R_800x600, R_1024x768, R_1152x864, R_1280x800, R_1360x768, R_1280x1024, R_1600x900, R_1600x1200, R_1920x1080
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

    public static bool IsSmallResolution { get { return ResolutionMode == ResolutionSettings.R_800x600; } }

    public static TV_2DVECTOR GetResolution
    {
      get
      {
        switch (ResolutionMode)
        {
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
      string path = Path.Combine(Globals.DataPath, "settings.ini");
      if (File.Exists(path))
      {
        INIFile f = new INIFile(path);
        ResolutionMode = f.GetValue("General", "Resolution", ResolutionMode);
        GameDebug = f.GetValue("General", "Debug", GameDebug);
        FullScreenMode = f.GetValue("General", "FullScreen", FullScreenMode);
        ShowPerformance = f.GetValue("General", "ShowPerformance", ShowPerformance);

        engine.SoundManager.MasterMusicVolume = f.GetValue("Audio", "MusicVol", 1f);
        engine.SoundManager.MasterSFXVolume = f.GetValue("Audio", "SFXVol", 1f);

        SteeringSensitivity = f.GetValue("Controls", "SteeringSensitivity", SteeringSensitivity);

        if (f.HasSection("Keyboard"))
        {
          foreach (INIFile.INISection.INILine ln in f.GetSection("Keyboard").Lines)
          {
            if (ln.HasKey)
            {
              InputFunction fn = InputFunction.Registry.Get(ln.Key);
              if (fn != null)
              {
                int fkey = f.GetValue("Keyboard", ln.Key, -2);
                if (fkey != -2)
                  fn.Key = fkey;
              }
            }
          }
        }
      }

      ResolutionX = (int)GetResolution.x;
      ResolutionY = (int)GetResolution.y;
    }

    public static void SaveSettings(Engine engine)
    {
      string filepath = Path.Combine(Globals.DataPath, "settings.ini");

      if (!File.Exists(filepath))
        File.Create(filepath).Close();

      INIFile f = new INIFile(filepath);
      f.Reset();
      f.SetValue("General", "Resolution", ResolutionMode);
      f.SetValue("General", "Debug", GameDebug);
      f.SetValue("General", "FullScreen", FullScreenMode);
      f.SetValue("General", "ShowPerformance", ShowPerformance);

      f.SetValue("Audio", "MusicVol", engine.SoundManager.MasterMusicVolume);
      f.SetValue("Audio", "SFXVol", engine.SoundManager.MasterSFXVolume);

      f.SetValue("Controls", "SteeringSensitivity", SteeringSensitivity);

      foreach (InputFunction fn in InputFunction.Registry.Functions)
        if (fn.Name != null && fn.Name.Length > 0)
          f.SetValue("Keyboard", fn.Name, fn.Key);

      f.WriteToFile(filepath);
    }
  }
}
