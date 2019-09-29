using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class Surface002_04ATI : Groups.GroundSurface
  {
    internal Surface002_04ATI(Factory owner) : base(owner, "Surface002_04ATI")
    {
      MeshData = new MeshData(Name, @"surface\surface002_04.x");
    }
  }
}

