using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class Surface001_01ATI : Groups.StaticScene
  {
    internal Surface001_01ATI(Factory owner) : base(owner, "SURF00101", "SURF00101")
    {
      RenderData.CullDistance = 20000;
      MeshData = new MeshData(Name, @"surface\surface001_01.x", 4);
    }
  }
}

