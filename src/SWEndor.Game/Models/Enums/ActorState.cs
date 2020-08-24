namespace SWEndor.Game.Models
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

  /// <summary>
  /// Provides extension methods for ActorState enum
  /// </summary>
  public static class ActorStateExt
  {
    /// <summary>Returns if the given state is DYING</summary>
    public static bool IsDying(this ActorState state) { return state == ActorState.DYING; }

    /// <summary>Returns if the given state is DEAD</summary>
    public static bool IsDead(this ActorState state) { return state == ActorState.DEAD; }

    /// <summary>Returns if the given state is either DYING or DEAD</summary>
    public static bool IsDyingOrDead(this ActorState state) { return state < 0; }
  }
}
