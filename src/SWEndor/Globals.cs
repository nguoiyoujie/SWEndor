using SWEndor.Core;
using System;
using System.Collections.Generic;
using System.IO;

namespace SWEndor
{
  public static class Globals
  {
    public const int MajorVersion = 0;
    public const int MinorVersion = 0;

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

    // Constant
    public const float PI = 3.1415f;
    public const float Rad2Deg = 180 / PI;
    public const float Deg2Rad = PI / 180;
    public const float LaserSpeed = 3000f;
    public const int ActorLimit = 1000;
    public const float ImminentCollisionFactor = 1.5f;
    public const float AcquisitionRange = 7500;


    // Directories
    public static string BasePath = AppDomain.CurrentDomain.BaseDirectory;
    public static string LogPath = Path.Combine(BasePath, @"log\");

    public static string FontPath = Path.Combine(BasePath, @"assets\fonts\");
    public static string ImagePath = Path.Combine(BasePath, @"assets\images\");
    public static string ModelPath = Path.Combine(BasePath, @"assets\models\");
    public static string MusicPath = Path.Combine(BasePath, @"assets\music\");
    public static string SoundPath = Path.Combine(BasePath, @"assets\sounds\");
    public static string AtmospherePath = Path.Combine(BasePath, @"assets\atmosphere\");
    public static string LandscapePath = Path.Combine(BasePath, @"assets\landscape\");

    public static string DataPath = Path.Combine(BasePath, @"data\");
    public static string DataShadersPath = Path.Combine(BasePath, @"data\shaders\");

    public static string CustomScenarioPath = Path.Combine(DataPath, @"scenarios\");


    // Files
    public static string ActorTypeINIDirectory = Path.Combine(DataPath, @"actor\");
    public static string ExplosionTypeINIDirectory = Path.Combine(DataPath, @"explosion\");
    public static string ProjectileTypeINIDirectory = Path.Combine(DataPath, @"projectile\");


    
    public static string DynamicMusicINIPath = Path.Combine(DataPath, @"dynamicmusic.ini");
    public static string WeaponStatINIPath = Path.Combine(DataPath, @"weapons.ini");
    public static string WeaponLoadoutStatINIPath = Path.Combine(DataPath, @"weaponloadouts.ini");
    
    // Game Engine
    public static Engine Engine;


    public static void PreInit()
    {
      CreateDirectories();
    }

    public static Engine InitEngine()
    {
      Engine = new Engine();
      Engine.Init();
      return Engine;
    }

    public static void CreateDirectories()
    { 
      Directory.CreateDirectory(ImagePath);
      Directory.CreateDirectory(AtmospherePath);
      Directory.CreateDirectory(LandscapePath);
      Directory.CreateDirectory(ModelPath);
      Directory.CreateDirectory(MusicPath);
      Directory.CreateDirectory(SoundPath);
      Directory.CreateDirectory(DataPath);
      Directory.CreateDirectory(LogPath);

      Directory.CreateDirectory(ActorTypeINIDirectory);
      Directory.CreateDirectory(ExplosionTypeINIDirectory);
      Directory.CreateDirectory(ProjectileTypeINIDirectory);
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
