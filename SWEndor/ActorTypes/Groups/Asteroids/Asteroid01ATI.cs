using System.IO;

namespace SWEndor.Actors.Types
{
  public class Asteroid01ATI : AsteroidGroup
  {
    private static Asteroid01ATI _instance;
    public static Asteroid01ATI Instance()
    {
      if (_instance == null) { _instance = new Asteroid01ATI(); }
      return _instance;
    }

    private Asteroid01ATI() : base("Asteroid 01")
    {
      SourceMeshPath = Path.Combine(Globals.ModelPath, @"asteroids\asteroid01.x");
    }
  }
}

