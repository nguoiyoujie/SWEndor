using MTV3D65;
using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  internal class BWing_Bottom_WingATI : Groups.SpinningDebris
  {
    internal BWing_Bottom_WingATI(Factory owner) : base(owner, "BWWBOT", "BWing_Bottom_WingATI")
    {
      MeshData = new MeshData(Name, @"bwing\bwing_bottom_wing.x", 1, CONST_TV_BLENDINGMODE.TV_BLEND_ALPHA, "Burn");
    }
  }
}

