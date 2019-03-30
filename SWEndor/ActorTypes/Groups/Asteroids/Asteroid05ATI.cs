using System.IO;

namespace SWEndor.Actors.Types
{
  public class Asteroid05ATI : AsteroidGroup
  {
    private static Asteroid05ATI _instance;
    public static Asteroid05ATI Instance()
    {
      if (_instance == null) { _instance = new Asteroid05ATI(); }
      return _instance;
    }

    private Asteroid05ATI() : base("Asteroid 05")
    {
      SourceMeshPath = Path.Combine(Globals.ModelPath, @"asteroids\asteroid05.x");
    }
  }
}

