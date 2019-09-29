using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class Surface001_01ATI : Groups.StaticScene
  {
    internal Surface001_01ATI(Factory owner) : base(owner, "Surface001_01ATI")
    {
      RenderData.CullDistance = 20000;
      Scale = 4;

      MeshData = new MeshData(Name, @"surface\surface001_01.x");
    }
  }
}

