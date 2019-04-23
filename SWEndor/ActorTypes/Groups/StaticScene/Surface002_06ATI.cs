using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Surface002_06ATI : Groups.StaticScene
  {
    private static Surface002_06ATI _instance;
    public static Surface002_06ATI Instance()
    {
      if (_instance == null) { _instance = new Surface002_06ATI(); }
      return _instance;
    }

    private Surface002_06ATI() : base("Surface002_06ATI")
    {
      CollisionEnabled = true;
      ImpactDamage = 200;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"surface\surface002_06.x");
    }
  }
}

