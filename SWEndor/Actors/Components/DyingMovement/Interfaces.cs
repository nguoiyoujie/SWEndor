namespace SWEndor.Actors.Components
{
  public interface IDyingMoveComponent
  {
    void Initialize(ActorInfo actor);
    void Update(ActorInfo actor, float time);
  }
}
