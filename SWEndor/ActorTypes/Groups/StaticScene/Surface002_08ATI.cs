using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class Surface002_08ATI : Groups.GroundSurface
  {
    internal Surface002_08ATI(Factory owner) : base(owner, "Surface002_08ATI")
    {
      MeshData = new MeshData(Name, @"surface\surface002_08.x");
    }
  }
}

