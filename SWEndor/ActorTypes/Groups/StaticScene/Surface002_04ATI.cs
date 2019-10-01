using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class Surface002_04ATI : Groups.GroundSurface
  {
    internal Surface002_04ATI(Factory owner) : base(owner, "SURF00204", "SURF00204")
    {
      MeshData = new MeshData(Name, @"surface\surface002_04.x");
    }
  }
}

