using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public static class Globals
  {
    public const float PI = 3.1415f;
    public static float LaserSpeed = 3000f;

    public static string BasePath = AppDomain.CurrentDomain.BaseDirectory;
    public static string DebugPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"debug\");
    public static string SaveGamePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"SavedGames\");
    public static string LogPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Log\");
    public static string SettingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Settings\");
    public static string ImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Assets\images\");
    public static string ModelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Assets\models\");
    public static string MusicPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Assets\music\");
    public static string SoundPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Assets\sounds\");
    public static string ShaderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Assets\shaders\");

    public static void CheckDirectories()
    {
      Directory.CreateDirectory(BasePath);
      Directory.CreateDirectory(DebugPath);
      Directory.CreateDirectory(ImagePath);
      Directory.CreateDirectory(ModelPath);
      Directory.CreateDirectory(MusicPath);
      Directory.CreateDirectory(SoundPath);
      Directory.CreateDirectory(ShaderPath);
      Directory.CreateDirectory(SaveGamePath);
      Directory.CreateDirectory(SettingsPath);
      Directory.CreateDirectory(LogPath);
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
