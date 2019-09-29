using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class Surface002_00ATI : Groups.GroundSurface
  {
    internal Surface002_00ATI(Factory owner) : base(owner, "Surface002_00ATI")
    {
      MeshData = new MeshData(Name, @"surface\surface002_00.x");
    }
  }
}

