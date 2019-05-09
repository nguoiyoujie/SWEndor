using SWEndor.Actors;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Surface003_00ATI : Groups.GroundSurface
  {
    internal Surface003_00ATI(Factory owner) : base(owner, "Surface003_00ATI")
    {
      Scale = 4;
      SourceMeshPath = Path.Combine(Globals.ModelPath, @"surface\surface003.x");
    }
  }
}

