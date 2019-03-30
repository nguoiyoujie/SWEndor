using System.IO;

namespace SWEndor.Actors.Types
{
  public class BWing_WingATI : SpinningDebrisGroup
  {
    private static BWing_WingATI _instance;
    public static BWing_WingATI Instance()
    {
      if (_instance == null) { _instance = new BWing_WingATI(); }
      return _instance;
    }

    private BWing_WingATI() : base("BWing_WingATI")
    {
      SourceMeshPath = Path.Combine(Globals.ModelPath, @"bwing\bwing_wing.x");
    }
  }
}

