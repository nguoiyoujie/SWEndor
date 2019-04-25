using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class TIE_InterceptorWingATI : Groups.SpinningDebris
  {
    internal TIE_InterceptorWingATI(Factory owner) : base(owner, "TIE_InterceptorWingATI")
    {
      SourceMeshPath = Path.Combine(Globals.ModelPath, @"tie\tie_interceptor_right_left_wing.x");
    }
  }
}

