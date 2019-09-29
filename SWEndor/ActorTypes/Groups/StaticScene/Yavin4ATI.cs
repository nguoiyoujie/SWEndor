using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class Yavin4ATI : Groups.StaticScene
  {
    internal Yavin4ATI(Factory owner) : base(owner, "Yavin4")
    {
      MeshData = MeshDataDecorator.CreateAlphaTexturedWall(Name, 50000, "planets/yavin4.bmp", "planets/yavin4alpha.bmp");
    }
  }
}


