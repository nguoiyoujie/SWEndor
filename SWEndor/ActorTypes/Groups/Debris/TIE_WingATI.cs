using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class TIE_WingATI : Groups.SpinningDebris
  {
    internal TIE_WingATI(Factory owner) : base(owner, "TIE_WingATI")
    {
      SourceMeshPath = Path.Combine(Globals.ModelPath, @"tie\tie_right_left_wing.x");
    }
  }
}

