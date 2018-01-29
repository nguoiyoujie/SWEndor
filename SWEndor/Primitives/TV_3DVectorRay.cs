using MTV3D65;
using System;
using System.Collections.Generic;
using System.Text;

namespace SWEndor
{
  public struct TV_3DVectorRay
  {
    public TV_3DVECTOR Start;
    public TV_3DVECTOR End;

    public TV_3DVectorRay(TV_3DVECTOR start, TV_3DVECTOR end)
    {
      Start = start;
      End = end;
    }
  }
}
