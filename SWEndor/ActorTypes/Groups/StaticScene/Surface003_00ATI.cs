using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class Surface003_00ATI : Groups.GroundSurface
  {
    internal Surface003_00ATI(Factory owner) : base(owner, "Surface003_00ATI")
    {
      Scale = 4;
      MeshData = new MeshData(Name, @"surface\surface003.x");
    }
  }
}

