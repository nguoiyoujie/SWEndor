using SWEndor.Actors.Components;
using System.IO;

namespace SWEndor.Actors.Types
{
  public class BWing_Top_WingATI : SpinningDebrisGroup
  {
    private static BWing_Top_WingATI _instance;
    public static BWing_Top_WingATI Instance()
    {
      if (_instance == null) { _instance = new BWing_Top_WingATI(); }
      return _instance;
    }

    private BWing_Top_WingATI() : base("BWing_Top_WingATI")
    {
      SourceMeshPath = Path.Combine(Globals.ModelPath, @"bwing\bwing_top_wing.x");
    }
  }
}

