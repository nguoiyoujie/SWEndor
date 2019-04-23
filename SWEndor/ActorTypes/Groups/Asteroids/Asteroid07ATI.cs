using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Asteroid07ATI : Groups.Asteroid
  {
    private static Asteroid07ATI _instance;
    public static Asteroid07ATI Instance()
    {
      if (_instance == null) { _instance = new Asteroid07ATI(); }
      return _instance;
    }

    private Asteroid07ATI() : base("Asteroid 07")
    {
      SourceMeshPath = Path.Combine(Globals.ModelPath, @"asteroids\asteroid07.x");
    }
  }
}

