using System.IO;

namespace SWEndor.Actors.Types
{
  public class XWing_RU_LD_WingATI : SpinningDebrisGroup
  {
    private static XWing_RU_LD_WingATI _instance;
    public static XWing_RU_LD_WingATI Instance()
    {
      if (_instance == null) { _instance = new XWing_RU_LD_WingATI(); }
      return _instance;
    }

    private XWing_RU_LD_WingATI() : base("XWing_RU_LD_WingATI")
    {
      SourceMeshPath = Path.Combine(Globals.ModelPath, @"xwing\xwing_rightup_leftdown_wing.x");
    }
  }
}

