using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class XWing_RU_LD_WingATI : Groups.SpinningDebris
  {
    internal XWing_RU_LD_WingATI(Factory owner) : base(owner, "XWing_RU_LD_WingATI")
    {
      SourceMeshPath = Path.Combine(Globals.ModelPath, @"xwing\xwing_rightup_leftdown_wing.x");
    }
  }
}

