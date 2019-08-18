namespace SWEndor.Actors
{
  /// <summary>
  /// Describe the state of an Actor as a game object
  /// </summary>
  public enum ActorState
  {
    // ALIVE STATES use Positive numbers

    /// <summary>
    /// Actor is executing normal game functions
    /// </summary>
    NORMAL = 0,

    /// <summary>
    /// Actor is in the free state: No speed limit (Note: consider a different setting to turn off Speed limits)
    /// </summary>
    FREE = 1,

    /// <summary>
    /// Actor is executing a hyperspace speed up or slow down
    /// </summary>
    HYPERSPACE = 2,

    // DEAD STATES use Negative numbers

    /// <summary>
    /// Actor is dying, and will be dead soon
    /// </summary>
    DYING = -1,

    /// <summary>
    /// Actor is dead and will be ripe for disposal
    /// </summary>
    DEAD = -2
  }

  public static class ActorStateHelper
  {
    public static bool IsDying(this ActorState state) { return state == ActorState.DYING; }

    public static bool IsDead(this ActorState state) { return state == ActorState.DEAD; }

    public static bool IsDyingOrDead(this ActorState state) { return state < 0; }
  }
}
