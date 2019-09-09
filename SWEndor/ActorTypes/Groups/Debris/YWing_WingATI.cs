using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class YWing_WingATI : Groups.SpinningDebris
  {
    internal YWing_WingATI(Factory owner) : base(owner, "YWing_WingATI")
    {
      MeshData = new MeshData(Name, @"ywing\ywing_right_left_wing.x");
    }
  }
}

