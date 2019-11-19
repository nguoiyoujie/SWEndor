namespace SWEndor.Models
{
  /// <summary>
  /// Describe the state of an Actor as handled by its Factory.
  /// </summary>
  public enum CreationState
  {
    /// <summary>
    /// Actor is not yet generated.
    /// </summary>
    PLANNED = 0,
    RESERVED_PLANNED = 10,

    /// <summary>
    /// Actor is generated, but is not yet eligible for game operations 
    /// </summary>
    GENERATED = 1,
    RESERVED_GENERATED = 11,

    /// <summary>
    /// Actor is in a valid state for running game operations.
    /// </summary>
    ACTIVE = 2,
    RESERVED_ACTIVE = 12,

    /// <summary>
    /// Actor is marked for clean-up
    /// </summary>
    PREDISPOSE = -1,

    /// <summary>
    /// Actor is destroyed and awaits clean-up
    /// </summary>
    DISPOSING = -2,

    /// <summary>
    /// Actor is destroyed and awaits clean-up / reuse
    /// </summary>
    DISPOSED = -3
  }
}
