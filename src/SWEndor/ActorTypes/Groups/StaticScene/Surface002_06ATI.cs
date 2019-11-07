using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class Surface002_06ATI : Groups.GroundSurface
  {
    internal Surface002_06ATI(Factory owner) : base(owner, "SURF00206", "SURF00206")
    {
      MeshData = new MeshData(Name, @"surface\surface002_06.x");
    }
  }
}

