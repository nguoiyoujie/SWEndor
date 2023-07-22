namespace SWEndor.Game.Actors.Models
{
  /// <summary>
  /// Defines the state of each subsystem component
  /// </summary>
  public enum SystemState : byte // 4 values 
  {
    /// <summary>The subsystem is not part of the active instruments</summary>
    PASSIVE = 0,

    /// <summary>The subsystem is present but destroyed</summary>
    DAMAGED = 1,

    /// <summary>The subsystem is present but disabled</summary>
    DISABLED = 2,

    /// <summary>The subsystem is present and functional</summary>
    ACTIVE = 3
  }
}
