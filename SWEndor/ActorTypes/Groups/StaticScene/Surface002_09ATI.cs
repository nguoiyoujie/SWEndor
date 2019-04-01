using System.IO;

namespace SWEndor.ActorTypes
{
  public class Surface002_09ATI : StaticSceneGroup
  {
    private static Surface002_09ATI _instance;
    public static Surface002_09ATI Instance()
    {
      if (_instance == null) { _instance = new Surface002_09ATI(); }
      return _instance;
    }

    private Surface002_09ATI() : base("Surface002_09ATI")
    {
      CollisionEnabled = true;
      ImpactDamage = 200;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"surface\surface002_09.x");
    }
  }
}

