using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Surface002_03ATI : Groups.GroundSurface
  {
    internal Surface002_03ATI(Factory owner) : base(owner, "Surface002_03ATI")
    {
      SourceMeshPath = Path.Combine(Globals.ModelPath, @"surface\surface002_03.x");
    }
  }
}

