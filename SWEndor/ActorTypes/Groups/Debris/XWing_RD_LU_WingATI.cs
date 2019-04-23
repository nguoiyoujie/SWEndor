using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class XWing_RD_LU_WingATI : Groups.SpinningDebris
  {
    private static XWing_RD_LU_WingATI _instance;
    public static XWing_RD_LU_WingATI Instance()
    {
      if (_instance == null) { _instance = new XWing_RD_LU_WingATI(); }
      return _instance;
    }

    private XWing_RD_LU_WingATI() : base("XWing_RD_LU_WingATI")
    {
      SourceMeshPath = Path.Combine(Globals.ModelPath, @"xwing\xwing_rightdown_leftup_wing.x");
    }
  }
}

