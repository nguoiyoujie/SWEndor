using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class YWing_WingATI : Groups.SpinningDebris
  {
    internal YWing_WingATI(Factory owner) : base(owner, "YWWING", "YWing_WingATI")
    {
      MeshData = new MeshData(Name, @"ywing\ywing_right_left_wing.x", 1, "Burn");
    }
  }
}

