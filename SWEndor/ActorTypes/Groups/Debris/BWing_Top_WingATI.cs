using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class BWing_Top_WingATI : Groups.SpinningDebris
  {
    internal BWing_Top_WingATI(Factory owner) : base(owner, "BWing_Top_WingATI")
    {
      SourceMeshPath = Path.Combine(Globals.ModelPath, @"bwing\bwing_top_wing.x");
    }
  }
}

