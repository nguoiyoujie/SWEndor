using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  internal class Surface002_05ATI : Groups.GroundSurface
  {
    internal Surface002_05ATI(Factory owner) : base(owner, "SURF00205", "SURF00205")
    {
      MeshData = new MeshData(Name, @"surface\surface002_05.x");
    }
  }
}

