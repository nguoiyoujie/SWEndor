using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Tower02_RuinsATI : Groups.Debris
  {
    private static Tower02_RuinsATI _instance;
    public static Tower02_RuinsATI Instance()
    {
      if (_instance == null) { _instance = new Tower02_RuinsATI(); }
      return _instance;
    }

    private Tower02_RuinsATI() : base("Turbolaser Tower 02 Ruins")
    {
      CullDistance = 10000;
      SourceMeshPath = Path.Combine(Globals.ModelPath, @"towers\tower_02_destroyed.x");
    }
  }
}

