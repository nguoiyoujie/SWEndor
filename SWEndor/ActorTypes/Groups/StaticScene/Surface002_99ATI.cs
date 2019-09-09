using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Surface002_99ATI : Groups.GroundSurface
  {
    internal Surface002_99ATI(Factory owner) : base(owner, "Surface002_99ATI")
    {
      MeshData = new MeshData(Name, @"surface\surface002_99.x");
    }
  }
}

