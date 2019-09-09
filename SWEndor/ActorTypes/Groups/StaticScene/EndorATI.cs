using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class EndorATI : Groups.StaticScene
  {
    internal EndorATI(Factory owner) : base(owner, "Endor")
    {
      MeshData = MeshDataDecorator.CreateTexturedModel(Name, "planet/endor.x", "planets/endor.jpg");
    }
  }
}


