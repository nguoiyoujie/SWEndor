using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class Surface002_03ATI : Groups.GroundSurface
  {
    internal Surface002_03ATI(Factory owner) : base(owner, "Surface002_03ATI")
    {
      MeshData = new MeshData(Name, @"surface\surface002_03.x");
    }
  }
}

