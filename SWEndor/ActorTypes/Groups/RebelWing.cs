using MTV3D65;
using SWEndor.ActorTypes.Components;
using SWEndor.Sound;

namespace SWEndor.ActorTypes.Groups
{
  public class RebelWing : Fighter
  {
    internal RebelWing(Factory owner, string name): base(owner, name)
    {
      RenderData.CullDistance = 7500;
    }
  }
}

