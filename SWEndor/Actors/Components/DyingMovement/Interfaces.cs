using SWEndor.Actors.Data;

namespace SWEndor.Actors.Components
{
  public interface IDyingMoveComponent
  {
    void Initialize(ActorInfo actor, ref MoveData data);
    void Update(ActorInfo actor, ref MoveData data, float time);
  }
}
