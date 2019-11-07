using MTV3D65;
using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class TIE_WingATI : Groups.SpinningDebris
  {
    internal TIE_WingATI(Factory owner) : base(owner, "TIEWING", "TIE_WingATI")
    {
      MeshData = new MeshData(Name, @"tie\tie_right_left_wing.x", 1, CONST_TV_BLENDINGMODE.TV_BLEND_ALPHA, "Burn");
    }
  }
}

