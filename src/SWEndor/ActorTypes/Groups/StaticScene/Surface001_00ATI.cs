using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  internal class Surface001_00ATI : Groups.StaticScene
  {
    internal Surface001_00ATI(Factory owner) : base(owner, "SURF00100", "SURF00100")
    {
      RenderData.CullDistance = 20000;
      MeshData = new MeshData(Name, @"surface\surface001_00.x", 4);
    }
  }
}

