using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  internal class Surface002_04ATI : Groups.GroundSurface
  {
    internal Surface002_04ATI(Factory owner) : base(owner, "SURF00204", "SURF00204")
    {
      MeshData = new MeshData(Engine, Name, @"surface\surface002_04.x");
    }
  }
}

