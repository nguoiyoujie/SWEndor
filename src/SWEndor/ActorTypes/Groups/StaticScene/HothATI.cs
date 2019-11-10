using MTV3D65;
using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  internal class HothATI : Groups.StaticScene
  {
    internal HothATI(Factory owner) : base(owner, "HOTH", "Hoth")
    {
      MeshData = MeshDataDecorator.CreateTexturedModel(Name, "planet/endor.x", "planets/hoth.bmp", CONST_TV_BLENDINGMODE.TV_BLEND_ADD);
    }
  }
}


