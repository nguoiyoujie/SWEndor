using MTV3D65;
using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  internal class XWing_RD_LU_WingATI : Groups.SpinningDebris
  {
    internal XWing_RD_LU_WingATI(Factory owner) : base(owner, "XWRDLU", "XWing_RD_LU_WingATI")
    {
      MeshData = new MeshData(Engine, Name, @"xwing\xwing_rightdown_leftup_wing.x", 1, CONST_TV_BLENDINGMODE.TV_BLEND_ALPHA, "Burn");
    }
  }
}

