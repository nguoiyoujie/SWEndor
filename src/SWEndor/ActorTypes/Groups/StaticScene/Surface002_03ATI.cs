using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  internal class Surface002_03ATI : Groups.GroundSurface
  {
    internal Surface002_03ATI(Factory owner) : base(owner, "SURF00203", "SURF00203")
    {
      MeshData = new MeshData(Name, @"surface\surface002_03.x");
    }
  }
}

