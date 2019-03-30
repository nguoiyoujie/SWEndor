using System.IO;

namespace SWEndor.Actors.Types
{
  public class Asteroid08ATI : AsteroidGroup
  {
    private static Asteroid08ATI _instance;
    public static Asteroid08ATI Instance()
    {
      if (_instance == null) { _instance = new Asteroid08ATI(); }
      return _instance;
    }

    private Asteroid08ATI() : base("Asteroid 08")
    {
      SourceMeshPath = Path.Combine(Globals.ModelPath, @"asteroids\asteroid08.x");
    }
  }
}

