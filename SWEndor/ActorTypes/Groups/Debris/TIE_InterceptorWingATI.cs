using MTV3D65;
using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class TIE_InterceptorWingATI : Groups.SpinningDebris
  {
    internal TIE_InterceptorWingATI(Factory owner) : base(owner, "TIEIWING", "TIE_InterceptorWingATI")
    {
      MeshData = new MeshData(Name, @"tie\tie_interceptor_right_left_wing.x", 1, CONST_TV_BLENDINGMODE.TV_BLEND_ALPHA, "Burn");
    }
  }
}

