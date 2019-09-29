using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class Surface001_00ATI : Groups.StaticScene
  {
    internal Surface001_00ATI(Factory owner) : base(owner, "Surface001_00ATI")
    {
      RenderData.CullDistance = 20000;
      Scale = 4;

      MeshData = new MeshData(Name, @"surface\surface001_00.x");
    }
  }
}

