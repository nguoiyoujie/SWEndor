using SWEndor.Actors.Components;
using System.IO;

namespace SWEndor.ActorTypes
{
  public class BWing_Bottom_WingATI : SpinningDebrisGroup
  {
    private static BWing_Bottom_WingATI _instance;
    public static BWing_Bottom_WingATI Instance()
    {
      if (_instance == null) { _instance = new BWing_Bottom_WingATI(); }
      return _instance;
    }

    private BWing_Bottom_WingATI() : base("BWing_Bottom_WingATI")
    {
      SourceMeshPath = Path.Combine(Globals.ModelPath, @"bwing\bwing_bottom_wing.x");
    }
  }
}

