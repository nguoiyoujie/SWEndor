using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  internal class Surface002_02ATI : Groups.GroundSurface
  {
    internal Surface002_02ATI(Factory owner) : base(owner, "SURF00202", "SURF00202")
    {
      MeshData = new MeshData(Engine, Name, @"surface\surface002_02.x");
    }
  }
}

