using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class BWing_Bottom_WingATI : Groups.SpinningDebris
  {
    internal BWing_Bottom_WingATI(Factory owner) : base(owner, "BWing_Bottom_WingATI")
    {
      MeshData = new MeshData(Name, @"bwing\bwing_bottom_wing.x");
    }
  }
}

