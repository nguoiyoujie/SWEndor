namespace SWEndor.Actors
{
  /// <summary>
  /// Describe the state of an Actor as a game object
  /// </summary>
  public enum ActorState
  {
    /// <summary>
    /// Actor is in a fixed position (Note: consider checking if TypeInfo.NoMove is sufficient?)
    /// </summary>
    FIXED,

    /// <summary>
    /// Actor is in the free state: No speed limit (Note: consider a different setting to turn off Speed limits)
    /// </summary>
    FREE,

    /// <summary>
    /// Actor is executing a hyperspace speed up or slow down
    /// </summary>
    HYPERSPACE,

    /// <summary>
    /// Actor is executing normal game functions
    /// </summary>
    NORMAL,

    /// <summary>
    /// Actor is dying, and will be dead soon
    /// </summary>
    DYING,

    /// <summary>
    /// Actor is dead and will be ripe for disposal
    /// </summary>
    DEAD
  }
}
