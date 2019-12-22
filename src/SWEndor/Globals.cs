using SWEndor.Core;
using System;
using System.Collections.Generic;
using System.IO;

namespace SWEndor
{
  /// <summary>
  /// Defines the global scopes
  /// </summary>
  public static class ScopeGlobals
  {
    internal const byte GLOBAL_TVSCENE = 113;
  }

  /// <summary>
  /// Defines the global values
  /// </summary>
  public static class Globals
  {
    /// <summary>The major version of the application</summary>
    public const int MajorVersion = 0;

    /// <summary>The minor version of the application</summary>
    public const int MinorVersion = 0;

    /// <summary>The application version</summary>
    public static string Version = 
        string.Format("v{0}.{1}.{2}-{3}"
        , MajorVersion
        , MinorVersion
        , DateTime.Parse(Build.Time).ToString("yyyy.MMdd.HHmm")
#if DEBUG
        , "Debug"
#else
        , "Release"
#endif
        );

    /// <summary>The value of PI</summary>
    public const float PI = 3.1415f;

    /// <summary>The conversion ratio from radian measure to degree measure</summary>
    public const float Rad2Deg = 180 / PI;

    /// <summary>The conversion ratio from degree measure to radian measure</summary>
    public const float Deg2Rad = PI / 180;

    /// <summary>The laser speed</summary>
    public const float LaserSpeed = 3000f;

    /// <summary>The factory object limit</summary>
    public const int ActorLimit = 1000;

    /// <summary>The collision factor as a multipler to object speed</summary>
    public const float ImminentCollisionFactor = 1.5f;

    /// <summary>The targeter acquisition range</summary>
    public const float AcquisitionRange = 7500;


    // Directories
    internal static string BasePath = AppDomain.CurrentDomain.BaseDirectory;
    internal static string LogPath = Path.Combine(BasePath, @"log\");
    internal static string ScreenshotsPath = Path.Combine(BasePath, @"screenshots\");

    internal static string FontPath = Path.Combine(BasePath, @"assets\fonts\");
    internal static string ImagePath = Path.Combine(BasePath, @"assets\images\");
    internal static string ModelPath = Path.Combine(BasePath, @"assets\models\");
    internal static string MusicPath = Path.Combine(BasePath, @"assets\music\");
    internal static string SoundPath = Path.Combine(BasePath, @"assets\sounds\");
    internal static string AtmospherePath = Path.Combine(BasePath, @"assets\atmosphere\");
    internal static string LandscapePath = Path.Combine(BasePath, @"assets\landscape\");

    internal static string DataPath = Path.Combine(BasePath, @"data\");
    internal static string DataShadersPath = Path.Combine(BasePath, @"data\shaders\");

    internal static string CustomScenarioPath = Path.Combine(DataPath, @"scenarios\");
    internal static string ActorTypeINIDirectory = Path.Combine(DataPath, @"actors\");
    internal static string ExplosionTypeINIDirectory = Path.Combine(DataPath, @"explosions\");
    internal static string ProjectileTypeINIDirectory = Path.Combine(DataPath, @"projectiles\");
    internal static string WeaponINIDirectory = Path.Combine(DataPath, @"weapons\");


    // Files
    internal static string DynamicMusicINIPath = Path.Combine(DataPath, @"dynamicmusic.ini");

    internal static string WeapAimINIPath = Path.Combine(WeaponINIDirectory, @"aim.ini");
    internal static string WeapAmmoINIPath = Path.Combine(WeaponINIDirectory, @"ammo.ini");
    internal static string WeapLoadINIPath = Path.Combine(WeaponINIDirectory, @"load.ini");
    internal static string WeapPortINIPath = Path.Combine(WeaponINIDirectory, @"port.ini");
    internal static string WeapProjINIPath = Path.Combine(WeaponINIDirectory, @"proj.ini");
    internal static string WeapTgtINIPath = Path.Combine(WeaponINIDirectory, @"tgt.ini");

    // Extensions
    //internal static string CustomScenarioExt = "*.scen";
    internal static string INIExt = "*.ini";
    internal static string ShaderFxExt = "*.fx";
    internal static string SoundExt = "*.wav";
    internal static string MusicExt = "*.mp3";

    // Game Engine
    internal static Engine Engine;

    internal static void PreInit()
    {
      CreateDirectories();
    }

    internal static Engine InitEngine()
    {
      Engine = new Engine();
      Engine.Init();
      return Engine;
    }

    internal static void CreateDirectories()
    { 
      Directory.CreateDirectory(ImagePath);
      Directory.CreateDirectory(AtmospherePath);
      Directory.CreateDirectory(LandscapePath);
      Directory.CreateDirectory(ModelPath);
      Directory.CreateDirectory(MusicPath);
      Directory.CreateDirectory(SoundPath);
      Directory.CreateDirectory(DataPath);
      Directory.CreateDirectory(LogPath);
      Directory.CreateDirectory(ScreenshotsPath);

      Directory.CreateDirectory(ActorTypeINIDirectory);
      Directory.CreateDirectory(ExplosionTypeINIDirectory);
      Directory.CreateDirectory(ProjectileTypeINIDirectory);
      Directory.CreateDirectory(WeaponINIDirectory);
    }

    internal static List<string> LoadingFlavourTexts = new List<string> {
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
