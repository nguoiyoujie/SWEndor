using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  internal class Surface002_00ATI : Groups.GroundSurface
  {
    internal Surface002_00ATI(Factory owner) : base(owner, "SURF00200", "SURF00200")
    {
      MeshData = new MeshData(Name, @"surface\surface002_00.x");
    }
  }
}

