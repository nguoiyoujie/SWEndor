using System.IO;

namespace SWEndor.Actors.Types
{
  public class Tower00_RuinsATI : DebrisGroup
  {
    private static Tower00_RuinsATI _instance;
    public static Tower00_RuinsATI Instance()
    {
      if (_instance == null) { _instance = new Tower00_RuinsATI(); }
      return _instance;
    }

    private Tower00_RuinsATI() : base("Turbolaser Tower 00 Ruins")
    {
      CullDistance = 10000;
      SourceMeshPath = Path.Combine(Globals.ModelPath, @"towers\tower_00_destroyed.x");
    }
  }
}

