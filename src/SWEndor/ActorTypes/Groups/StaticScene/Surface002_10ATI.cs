using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  internal class Surface002_10ATI : Groups.GroundSurface
  {
    internal Surface002_10ATI(Factory owner) : base(owner, "SURF00210", "SURF00210")
    {
      MeshData = new MeshData(Name, @"surface\surface002_10.x");
    }
  }
}

