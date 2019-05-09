using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Surface002_12ATI : Groups.GroundSurface
  {
    internal Surface002_12ATI(Factory owner) : base(owner, "Surface002_12ATI")
    {
      SourceMeshPath = Path.Combine(Globals.ModelPath, @"surface\surface002_12.x");
    }
  }
}

