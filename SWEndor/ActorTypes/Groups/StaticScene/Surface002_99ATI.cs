using System.IO;

namespace SWEndor.ActorTypes
{
  public class Surface002_99ATI : StaticSceneGroup
  {
    private static Surface002_99ATI _instance;
    public static Surface002_99ATI Instance()
    {
      if (_instance == null) { _instance = new Surface002_99ATI(); }
      return _instance;
    }

    private Surface002_99ATI() : base("Surface002_99ATI")
    {
      CollisionEnabled = true;
      ImpactDamage = 200;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"surface\surface002_99.x");
    }
  }
}

