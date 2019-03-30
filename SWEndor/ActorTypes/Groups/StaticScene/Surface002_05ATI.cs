using System.IO;

namespace SWEndor.Actors.Types
{
  public class Surface002_05ATI : StaticSceneGroup
  {
    private static Surface002_05ATI _instance;
    public static Surface002_05ATI Instance()
    {
      if (_instance == null) { _instance = new Surface002_05ATI(); }
      return _instance;
    }

    private Surface002_05ATI() : base("Surface002_05ATI")
    {
      CollisionEnabled = true;
      ImpactDamage = 200;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"surface\surface002_05.x");
    }
  }
}

