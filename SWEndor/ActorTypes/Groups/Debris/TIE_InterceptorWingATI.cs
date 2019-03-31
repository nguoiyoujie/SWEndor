using SWEndor.Actors.Components;
using System.IO;

namespace SWEndor.Actors.Types
{
  public class TIE_InterceptorWingATI : SpinningDebrisGroup
  {
    private static TIE_InterceptorWingATI _instance;
    public static TIE_InterceptorWingATI Instance()
    {
      if (_instance == null) { _instance = new TIE_InterceptorWingATI(); }
      return _instance;
    }

    private TIE_InterceptorWingATI() : base("TIE_InterceptorWingATI")
    {
      SourceMeshPath = Path.Combine(Globals.ModelPath, @"tie\tie_interceptor_right_left_wing.x");
    }
  }
}

