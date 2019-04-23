using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Surface002_12ATI : Groups.StaticScene
  {
    private static Surface002_12ATI _instance;
    public static Surface002_12ATI Instance()
    {
      if (_instance == null) { _instance = new Surface002_12ATI(); }
      return _instance;
    }

    private Surface002_12ATI() : base("Surface002_12ATI")
    {
      CollisionEnabled = true;
      ImpactDamage = 200;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"surface\surface002_12.x");
    }
  }
}

