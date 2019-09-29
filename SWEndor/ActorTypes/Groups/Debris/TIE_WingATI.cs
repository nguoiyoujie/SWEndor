using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class TIE_WingATI : Groups.SpinningDebris
  {
    internal TIE_WingATI(Factory owner) : base(owner, "TIE_WingATI")
    {
      MeshData = new MeshData(Name, @"tie\tie_right_left_wing.x");
    }
  }
}

