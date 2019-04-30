namespace SWEndor.Actors.Components
{
  public class DyingKill : IDyingMoveComponent
  {
    public static readonly DyingKill Instance = new DyingKill();
    private DyingKill() { }

    public void Initialize(ActorInfo actor) { actor.ActorState = ActorState.DEAD; }
    public void Update(ActorInfo actor, float time) { }
  }
}
