using MTV3D65;
using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  internal class XWing_RU_LD_WingATI : Groups.SpinningDebris
  {
    internal XWing_RU_LD_WingATI(Factory owner) : base(owner, "XWRULD", "XWing_RU_LD_WingATI")
    {
      MeshData = new MeshData(Engine, Name, @"xwing\xwing_rightup_leftdown_wing.x", 1, CONST_TV_BLENDINGMODE.TV_BLEND_ALPHA, "Burn");
    }
  }
}

