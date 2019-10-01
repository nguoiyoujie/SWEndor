using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class Surface002_11ATI : Groups.GroundSurface
  {
    internal Surface002_11ATI(Factory owner) : base(owner, "SURF00211", "SURF00211")
    {
      MeshData = new MeshData(Name, @"surface\surface002_11.x");
    }
  }
}

