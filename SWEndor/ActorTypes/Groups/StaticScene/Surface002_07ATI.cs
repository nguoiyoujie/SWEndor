using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Surface002_07ATI : Groups.GroundSurface
  {
    internal Surface002_07ATI(Factory owner) : base(owner, "Surface002_07ATI")
    {
      MeshData = new MeshData(Name, @"surface\surface002_07.x");
    }
  }
}

