using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Surface002_08ATI : Groups.StaticScene
  {
    private static Surface002_08ATI _instance;
    public static Surface002_08ATI Instance()
    {
      if (_instance == null) { _instance = new Surface002_08ATI(); }
      return _instance;
    }

    private Surface002_08ATI() : base("Surface002_08ATI")
    {
      CollisionEnabled = true;
      ImpactDamage = 200;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"surface\surface002_08.x");
    }
  }
}

