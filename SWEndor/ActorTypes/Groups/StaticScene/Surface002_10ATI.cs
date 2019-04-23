using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Surface002_10ATI : Groups.StaticScene
  {
    private static Surface002_10ATI _instance;
    public static Surface002_10ATI Instance()
    {
      if (_instance == null) { _instance = new Surface002_10ATI(); }
      return _instance;
    }

    private Surface002_10ATI() : base("Surface002_10ATI")
    {
      CollisionEnabled = true;
      ImpactDamage = 200;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"surface\surface002_10.x");
    }
  }
}

