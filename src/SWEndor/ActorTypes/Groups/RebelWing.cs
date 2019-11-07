using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Groups
{
  public class RebelWing : Fighter
  {
    internal RebelWing(Factory owner, string id, string name) : base(owner, id, name)
    {
      RenderData.CullDistance = 7500;
    }
  }
}

