using SWEndor.Actors.Data;

namespace SWEndor.Actors.Components
{
  public interface IMoveComponent
  {
    void Move(ActorInfo actor, ref MoveData data);
  }
}
