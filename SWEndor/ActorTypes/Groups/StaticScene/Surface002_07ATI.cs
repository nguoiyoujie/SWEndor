using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class Surface002_07ATI : Groups.GroundSurface
  {
    internal Surface002_07ATI(Factory owner) : base(owner, "SURF00207", "SURF00207")
    {
      MeshData = new MeshData(Name, @"surface\surface002_07.x");
    }
  }
}

