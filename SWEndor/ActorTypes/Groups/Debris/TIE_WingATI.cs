using System.IO;

namespace SWEndor.ActorTypes
{
  public class TIE_WingATI : SpinningDebrisGroup
  {
    private static TIE_WingATI _instance;
    public static TIE_WingATI Instance()
    {
      if (_instance == null) { _instance = new TIE_WingATI(); }
      return _instance;
    }

    private TIE_WingATI() : base("TIE_WingATI")
    {
      SourceMeshPath = Path.Combine(Globals.ModelPath, @"tie\tie_right_left_wing.x");
    }
  }
}

