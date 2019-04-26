using System;
using System.Collections.Generic;
using System.IO;

namespace SWEndor
{
  public static class Globals
  {
    public const int MajorVersion = 0;
    public const int MinorVersion = 0;

    public static string Version
    {
      get
      {
        return string.Format("v{0}.{1}.{2}"
        , MajorVersion
        , MinorVersion
        , DateTime.Parse(Build.Time).ToString("yyyy.MMdd.hhmm"));
      }
    }

    // Constant
    public const float PI = 3.1415f;
    public const float LaserSpeed = 3000f;

    // Directories
    public static string BasePath = AppDomain.CurrentDomain.BaseDirectory;
    public static string DllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"dll\");
    public static string DebugPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"debug\");
    public static string SaveGamePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"SavedGames\");
    public static string LogPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Log\");
    public static string ScenarioPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Scenarios\");
    public static string SettingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Settings\");
    public static string ImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Assets\images\");
    public static string ModelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Assets\models\");
    public static string MusicPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Assets\music\");
    public static string SoundPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Assets\sounds\");
    public static string AtmospherePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Assets\atmosphere\");
    public static string LandscapePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Assets\landscape\");
    public static string CustomScenarioPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Scenarios\");
    public static string DataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Data\");

    // Files
    public static string ActorTypeINIPath = Path.Combine(DataPath, @"actortypes.ini");
    public static string DynamicMusicINIPath = Path.Combine(DataPath, @"dynamicmusic.ini");
    public static string WeaponStatINIPath = Path.Combine(DataPath, @"weapons.ini");

    // Game Engine
    public static Engine Engine;


    public static void PreInit()
    {
      CreateDirectories();
      LoadDlls();
    }

    public static Engine InitEngine()
    {
      Engine = new Engine();
      Engine.Init();
      return Engine;
    }

    public static void CreateDirectories()
    { 
      Directory.CreateDirectory(BasePath);
      Directory.CreateDirectory(DllPath);
      Directory.CreateDirectory(DebugPath);
      Directory.CreateDirectory(ImagePath);
      Directory.CreateDirectory(AtmospherePath);
      Directory.CreateDirectory(LandscapePath);
      Directory.CreateDirectory(ModelPath);
      Directory.CreateDirectory(MusicPath);
      Directory.CreateDirectory(SoundPath);
      Directory.CreateDirectory(SaveGamePath);
      Directory.CreateDirectory(ScenarioPath);
      Directory.CreateDirectory(DataPath);
      Directory.CreateDirectory(SettingsPath);
      Directory.CreateDirectory(LogPath);
    }

    public static void UnloadDlls()
    {
      //foreach (string dllpath in Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll", SearchOption.TopDirectoryOnly))
      //  File.Delete(dllpath);
      string rmdllbat = Path.Combine(DllPath, "rmdll.bat");
      if (File.Exists(rmdllbat))
        System.Diagnostics.Process.Start(rmdllbat);
    }

    public static void LoadDlls()
    {
      foreach (string dllpath in Directory.GetFiles(DllPath, "*.dll", SearchOption.TopDirectoryOnly))
        File.Copy(dllpath, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.GetFileName(dllpath)), true);
    }

    public static List<string> LoadingFlavourTexts = new List<string> {
                                                                       "Learning the secrets of hyperdrive travel...",
                                                                       "Teaching a youngling Force Hyperdrive",
                                                                       "Attempting to configure hyperspace...",
                                                                       "Activating your computer's hyperdrive...",
                                                                       "Creating TIE factory...",
                                                                       "Deleting TIE factory...",
                                                                       "Setting up traps...",
                                                                       "Downloading Mr Bones...",
                                                                       "Blowing up main reactors...",
                                                                       "Rendering high ground...",
                                                                       "The Dark Side is strong in this machine.",
                                                                       "Preparing to fire when ready...",
                                                                       "Preparing parental controls...",
                                                                       "Executing Order 66...",
                                                                       "Spliting Maul...",
                                                                       "Universally expanding Expanded Universe...",
                                                                       "Deleting Rebel bases...",
                                                                       "Ramming Executor bridge",
                                                                       "There is no lightsabers in this game. Sorry.",
                                                                       "Conversing in Wookie",
                                                                       "Applying shields to TIE fighters"
                                                                        };
  }
}
