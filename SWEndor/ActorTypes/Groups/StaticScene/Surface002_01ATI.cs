using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Surface002_01ATI : Groups.GroundSurface
  {
    internal Surface002_01ATI(Factory owner) : base(owner, "Surface002_01ATI")
    {
      SourceMeshPath = Path.Combine(Globals.ModelPath, @"surface\surface002_01.x");
    }
  }
}

