using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  internal class Surface002_11ATI : Groups.GroundSurface
  {
    internal Surface002_11ATI(Factory owner) : base(owner, "SURF00211", "SURF00211")
    {
      MeshData = new MeshData(Engine, Name, @"surface\surface002_11.x");
    }
  }
}

