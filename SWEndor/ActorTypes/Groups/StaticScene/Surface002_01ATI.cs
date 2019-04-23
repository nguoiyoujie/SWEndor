using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Surface002_01ATI : Groups.StaticScene
  {
    private static Surface002_01ATI _instance;
    public static Surface002_01ATI Instance()
    {
      if (_instance == null) { _instance = new Surface002_01ATI(); }
      return _instance;
    }

    private Surface002_01ATI() : base("Surface002_01ATI")
    {
      CollisionEnabled = true;
      ImpactDamage = 200;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"surface\surface002_01.x");
    }
  }
}

