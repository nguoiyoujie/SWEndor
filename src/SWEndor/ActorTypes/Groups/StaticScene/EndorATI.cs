using MTV3D65;
using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  internal class EndorATI : Groups.StaticScene
  {
    internal EndorATI(Factory owner) : base(owner, "ENDOR", "Endor")
    {
      MeshData = MeshDataDecorator.CreateTexturedModel(Name, "planet/endor.x", "planets/endor.jpg", CONST_TV_BLENDINGMODE.TV_BLEND_ADD);
    }
  }
}


