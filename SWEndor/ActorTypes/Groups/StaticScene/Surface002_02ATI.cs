using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class Surface002_02ATI : Groups.GroundSurface
  {
    internal Surface002_02ATI(Factory owner) : base(owner, "SURF00202", "SURF00202")
    {
      MeshData = new MeshData(Name, @"surface\surface002_02.x");
    }
  }
}

