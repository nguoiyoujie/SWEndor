using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class XWing_RD_LU_WingATI : Groups.SpinningDebris
  {
    internal XWing_RD_LU_WingATI(Factory owner) : base(owner, "XWing_RD_LU_WingATI")
    {
      SourceMeshPath = Path.Combine(Globals.ModelPath, @"xwing\xwing_rightdown_leftup_wing.x");
    }
  }
}

