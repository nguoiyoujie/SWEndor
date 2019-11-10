using MTV3D65;
using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  internal class YavinATI : Groups.StaticScene
  {
    internal YavinATI(Factory owner) : base(owner, "YAVIN", "Yavin")
    {
      MeshData = MeshDataDecorator.CreateTexturedModel(Name, "planet/endor.x", "planets/yavin.bmp", CONST_TV_BLENDINGMODE.TV_BLEND_ADD);
    }
  }
}


