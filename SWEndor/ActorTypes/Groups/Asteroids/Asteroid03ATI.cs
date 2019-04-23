using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Asteroid03ATI : Groups.Asteroid
  {
    private static Asteroid03ATI _instance;
    public static Asteroid03ATI Instance()
    {
      if (_instance == null) { _instance = new Asteroid03ATI(); }
      return _instance;
    }

    private Asteroid03ATI() : base("Asteroid 03")
    {
      SourceMeshPath = Path.Combine(Globals.ModelPath, @"asteroids\asteroid03.x");
    }
  }
}

