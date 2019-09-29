using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class YavinATI : Groups.StaticScene
  {
    internal YavinATI(Factory owner) : base(owner, "Yavin")
    {
      MeshData = MeshDataDecorator.CreateTexturedModel(Name, "planet/endor.x", "planets/yavin.bmp");
    }
  }
}


