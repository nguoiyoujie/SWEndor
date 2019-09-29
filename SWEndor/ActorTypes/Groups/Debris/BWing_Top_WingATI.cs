using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class BWing_Top_WingATI : Groups.SpinningDebris
  {
    internal BWing_Top_WingATI(Factory owner) : base(owner, "BWing_Top_WingATI")
    {
      MeshData = new MeshData(Name, @"bwing\bwing_top_wing.x");
    }
  }
}

