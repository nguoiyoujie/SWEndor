using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class HothATI : Groups.StaticScene
  {
    internal HothATI(Factory owner) : base(owner, "Hoth")
    {
      MeshData = MeshDataDecorator.CreateTexturedModel(Name, "planet/endor.x", "planets/hoth.bmp");
    }
  }
}


