using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Surface002_00ATI : Groups.StaticScene
  {
    private static Surface002_00ATI _instance;
    public static Surface002_00ATI Instance()
    {
      if (_instance == null) { _instance = new Surface002_00ATI(); }
      return _instance;
    }

    private Surface002_00ATI() : base("Surface002_00ATI")
    {
      CollisionEnabled = true;
      ImpactDamage = 200;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"surface\surface002_00.x");
    }
  }
}

