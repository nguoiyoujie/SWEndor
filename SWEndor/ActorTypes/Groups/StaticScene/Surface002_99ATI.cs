using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class Surface002_99ATI : Groups.GroundSurface
  {
    internal Surface002_99ATI(Factory owner) : base(owner, "SURF00299", "SURF00299")
    {
      MeshData = new MeshData(Name, @"surface\surface002_99.x");
    }
  }
}

