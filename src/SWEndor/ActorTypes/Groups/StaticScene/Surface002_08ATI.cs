using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  internal class Surface002_08ATI : Groups.GroundSurface
  {
    internal Surface002_08ATI(Factory owner) : base(owner, "SURF00208", "SURF00208")
    {
      MeshData = new MeshData(Engine, Name, @"surface\surface002_08.x");
    }
  }
}

