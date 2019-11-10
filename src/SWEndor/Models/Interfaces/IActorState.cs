namespace SWEndor.Models
{
  /// <summary>
  /// Represents an object that supports an actor state
  /// </summary>
  public interface IActorState
  {
    ActorState ActorState { get; }
    void SetState_Dead();
  }
}