using MTV3D65;
using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  internal class BWing_WingATI : Groups.SpinningDebris
  {
    internal BWing_WingATI(Factory owner) : base(owner, "BWWING", "BWing_WingATI")
    {
      MeshData = new MeshData(Engine, Name, @"bwing\bwing_wing.x", 1, CONST_TV_BLENDINGMODE.TV_BLEND_ALPHA, "Burn");
    }
  }
}

