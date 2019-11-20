using MTV3D65;
using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  internal class TIE_WingATI : Groups.SpinningDebris
  {
    internal TIE_WingATI(Factory owner) : base(owner, "TIEWING", "TIE_WingATI")
    {
      MeshData = new MeshData(Engine, Name, @"tie\tie_right_left_wing.x", 1, CONST_TV_BLENDINGMODE.TV_BLEND_ALPHA, "Burn");
    }
  }
}

