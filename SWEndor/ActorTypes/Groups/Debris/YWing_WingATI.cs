using SWEndor.Actors.Components;
using System.IO;

namespace SWEndor.ActorTypes
{
  public class YWing_WingATI : SpinningDebrisGroup
  {
    private static YWing_WingATI _instance;
    public static YWing_WingATI Instance()
    {
      if (_instance == null) { _instance = new YWing_WingATI(); }
      return _instance;
    }

    private YWing_WingATI() : base("YWing_WingATI")
    {
      SourceMeshPath = Path.Combine(Globals.ModelPath, @"ywing\ywing_right_left_wing.x");
    }
  }
}

