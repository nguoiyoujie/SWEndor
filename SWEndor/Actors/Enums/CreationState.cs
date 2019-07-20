namespace SWEndor.Actors
{
  /// <summary>
  /// Describe the state of an Actor as handled by its Factory.
  /// </summary>
  public enum CreationState
  {
    /// <summary>
    /// Actor is not yet generated.
    /// </summary>
    PLANNED, 

    /// <summary>
    /// Actor is generated, but is not yet eligible for game operations 
    /// </summary>
    GENERATED,

    /// <summary>
    /// Actor is in a valid state for running game operations.
    /// </summary>
    ACTIVE,

    /// <summary>
    /// Actor is destroyed and awaits clean-up
    /// </summary>
    DISPOSING,

    /// <summary>
    /// Actor is disposed and awaits reuse
    /// </summary>
    DISPOSED
  }
}
