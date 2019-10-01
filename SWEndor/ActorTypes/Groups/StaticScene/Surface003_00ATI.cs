using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class Surface003_00ATI : Groups.GroundSurface
  {
    internal Surface003_00ATI(Factory owner) : base(owner, "SURF00300", "SURF00300")
    {
      MeshData = new MeshData(Name, @"surface\surface003.x", 4);
    }
  }
}

