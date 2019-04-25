using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class YWing_WingATI : Groups.SpinningDebris
  {
    internal YWing_WingATI(Factory owner) : base(owner, "YWing_WingATI")
    {
      SourceMeshPath = Path.Combine(Globals.ModelPath, @"ywing\ywing_right_left_wing.x");
    }
  }
}

