using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class Surface002_10ATI : Groups.GroundSurface
  {
    internal Surface002_10ATI(Factory owner) : base(owner, "Surface002_10ATI")
    {
      MeshData = new MeshData(Name, @"surface\surface002_10.x");
    }
  }
}

