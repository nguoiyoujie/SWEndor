using MTV3D65;
using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  internal class YWing_WingATI : Groups.SpinningDebris
  {
    internal YWing_WingATI(Factory owner) : base(owner, "YWWING", "YWing_WingATI")
    {
      MeshData = new MeshData(Engine, Name, @"ywing\ywing_right_left_wing.x", 1, CONST_TV_BLENDINGMODE.TV_BLEND_ALPHA, "Burn");
    }
  }
}

