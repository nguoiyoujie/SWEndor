﻿using MTV3D65;
using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class BWing_Top_WingATI : Groups.SpinningDebris
  {
    internal BWing_Top_WingATI(Factory owner) : base(owner, "BWWTOP", "BWing_Top_WingATI")
    {
      MeshData = new MeshData(Name, @"bwing\bwing_top_wing.x", 1, CONST_TV_BLENDINGMODE.TV_BLEND_ALPHA, "Burn");
    }
  }
}

