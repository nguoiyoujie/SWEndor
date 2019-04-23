using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Tower03_RuinsATI : Groups.Debris
  {
    private static Tower03_RuinsATI _instance;
    public static Tower03_RuinsATI Instance()
    {
      if (_instance == null) { _instance = new Tower03_RuinsATI(); }
      return _instance;
    }

    private Tower03_RuinsATI() : base("Turbolaser Tower 03 Ruins")
    {
      CullDistance = 10000;
      SourceMeshPath = Path.Combine(Globals.ModelPath, @"towers\tower_03_destroyed.x");
    }
  }
}

