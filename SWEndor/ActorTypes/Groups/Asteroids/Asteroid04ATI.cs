using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Asteroid04ATI : Groups.Asteroid
  {
    private static Asteroid04ATI _instance;
    public static Asteroid04ATI Instance()
    {
      if (_instance == null) { _instance = new Asteroid04ATI(); }
      return _instance;
    }

    private Asteroid04ATI() : base("Asteroid 04")
    {
      SourceMeshPath = Path.Combine(Globals.ModelPath, @"asteroids\asteroid04.x");
    }
  }
}

