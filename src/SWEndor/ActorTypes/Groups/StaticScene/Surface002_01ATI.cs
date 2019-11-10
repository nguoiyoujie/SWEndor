using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  internal class Surface002_01ATI : Groups.GroundSurface
  {
    internal Surface002_01ATI(Factory owner) : base(owner, "SURF00201", "SURF00201")
    {
      MeshData = new MeshData(Name, @"surface\surface002_01.x");
    }
  }
}

