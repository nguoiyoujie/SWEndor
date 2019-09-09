using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class BWing_WingATI : Groups.SpinningDebris
  {
    internal BWing_WingATI(Factory owner) : base(owner, "BWing_WingATI")
    {
      MeshData = new MeshData(Name, @"bwing\bwing_wing.x");
    }
  }
}

