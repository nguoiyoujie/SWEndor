using MTV3D65;
using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  internal class Yavin4ATI : Groups.StaticScene
  {
    internal Yavin4ATI(Factory owner) : base(owner, "YAVIN4", "Yavin4")
    {
      MeshData = MeshDataDecorator.CreateAlphaTexturedWall(Engine, Name, 50000, "planets/yavin4.bmp", "planets/yavin4alpha.bmp", CONST_TV_BLENDINGMODE.TV_BLEND_ADD);
    }
  }
}


