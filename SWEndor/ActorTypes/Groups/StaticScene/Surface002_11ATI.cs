using System.IO;

namespace SWEndor.Actors.Types
{
  public class Surface002_11ATI : StaticSceneGroup
  {
    private static Surface002_11ATI _instance;
    public static Surface002_11ATI Instance()
    {
      if (_instance == null) { _instance = new Surface002_11ATI(); }
      return _instance;
    }

    private Surface002_11ATI() : base("Surface002_11ATI")
    {
      CollisionEnabled = true;
      ImpactDamage = 200;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"surface\surface002_11.x");
    }
  }
}

