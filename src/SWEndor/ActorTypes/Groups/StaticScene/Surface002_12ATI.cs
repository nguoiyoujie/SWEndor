using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class Surface002_12ATI : Groups.GroundSurface
  {
    internal Surface002_12ATI(Factory owner) : base(owner, "SURF00212", "SURF00212")
    {
      MeshData = new MeshData(Name, @"surface\surface002_12.x");
    }
  }
}

