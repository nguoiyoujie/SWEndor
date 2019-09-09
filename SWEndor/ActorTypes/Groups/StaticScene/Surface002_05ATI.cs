using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Surface002_05ATI : Groups.GroundSurface
  {
    internal Surface002_05ATI(Factory owner) : base(owner, "Surface002_05ATI")
    {
      MeshData = new MeshData(Name, @"surface\surface002_05.x");
    }
  }
}

