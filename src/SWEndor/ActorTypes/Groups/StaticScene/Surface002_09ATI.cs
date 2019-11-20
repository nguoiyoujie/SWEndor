using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  internal class Surface002_09ATI : Groups.GroundSurface
  {
    internal Surface002_09ATI(Factory owner) : base(owner, "SURF00209", "SURF00209")
    {
      MeshData = new MeshData(Engine, Name, @"surface\surface002_09.x");
    }
  }
}

