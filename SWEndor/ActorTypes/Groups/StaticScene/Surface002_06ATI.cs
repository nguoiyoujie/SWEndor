using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Surface002_06ATI : Groups.GroundSurface
  {
    internal Surface002_06ATI(Factory owner) : base(owner, "Surface002_06ATI")
    {
      SourceMeshPath = Path.Combine(Globals.ModelPath, @"surface\surface002_06.x");
    }
  }
}

