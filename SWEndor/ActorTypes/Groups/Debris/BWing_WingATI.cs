using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class BWing_WingATI : Groups.SpinningDebris
  {
    internal BWing_WingATI(Factory owner) : base(owner, "BWing_WingATI")
    {
      SourceMeshPath = Path.Combine(Globals.ModelPath, @"bwing\bwing_wing.x");
    }
  }
}

