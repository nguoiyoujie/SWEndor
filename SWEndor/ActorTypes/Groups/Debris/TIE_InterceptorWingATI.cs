using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class TIE_InterceptorWingATI : Groups.SpinningDebris
  {
    internal TIE_InterceptorWingATI(Factory owner) : base(owner, "TIE_InterceptorWingATI")
    {
      MeshData = new MeshData(Name, @"tie\tie_interceptor_right_left_wing.x");
    }
  }
}

