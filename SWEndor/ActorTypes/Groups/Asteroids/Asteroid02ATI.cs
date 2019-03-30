using System.IO;

namespace SWEndor.Actors.Types
{
  public class Asteroid02ATI : AsteroidGroup
  {
    private static Asteroid02ATI _instance;
    public static Asteroid02ATI Instance()
    {
      if (_instance == null) { _instance = new Asteroid02ATI(); }
      return _instance;
    }

    private Asteroid02ATI() : base("Asteroid 02")
    {
      SourceMeshPath = Path.Combine(Globals.ModelPath, @"asteroids\asteroid02.x");
    }
  }
}

