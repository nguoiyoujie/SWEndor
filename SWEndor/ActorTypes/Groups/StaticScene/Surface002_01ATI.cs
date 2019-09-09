using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Surface002_01ATI : Groups.GroundSurface
  {
    internal Surface002_01ATI(Factory owner) : base(owner, "Surface002_01ATI")
    {
      MeshData = new MeshData(Name, @"surface\surface002_01.x");
    }
  }
}

