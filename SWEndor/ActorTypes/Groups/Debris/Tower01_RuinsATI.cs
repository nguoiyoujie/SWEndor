using System.IO;

namespace SWEndor.Actors.Types
{
  public class Tower01_RuinsATI : DebrisGroup
  {
    private static Tower01_RuinsATI _instance;
    public static Tower01_RuinsATI Instance()
    {
      if (_instance == null) { _instance = new Tower01_RuinsATI(); }
      return _instance;
    }

    private Tower01_RuinsATI() : base("Turbolaser Tower 01 Ruins")
    {
      CullDistance = 10000;
      SourceMeshPath = Path.Combine(Globals.ModelPath, @"towers\tower_01_destroyed.x");
    }
  }
}

