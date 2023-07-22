using System;

namespace SWEndor.Game.Models
{
  /// <summary>
  /// The 'type' of an Actor for targeting purposes
  /// </summary>
  [Flags]
  public enum TargetExclusionState
  {
    NONE = 0,

    /// <summary>
    /// Exclude stunned actors as targets
    /// </summary>
    STUNNED = 0x0001,

    /// <summary>
    /// Exclude fighters with missile lock as targets
    /// </summary>
    FIGHTER_MISSILE_LOCKED = 0x0002,

    /// <summary>
    /// Exclude ships with missile lock as targets
    /// </summary>
    SHIP_MISSILE_LOCKED = 0x0004,
  }

  /// <summary>
  /// Provides extension methods for TargetExclusionState enum
  /// </summary>
  public static class TargetExclusionStateExt
  {
    /// <summary>Returns whether a target type contains a subset</summary>
    /// <param name="mask">The mask to compare</param>
    /// <param name="subset">The subset to compare</param>
    /// <returns>Returns true if the mask contains all bits of the subset, false if otherwise</returns>
    public static bool Has(this TargetExclusionState mask, TargetExclusionState subset) { return (mask & subset) == subset; }

    /// <summary>Returns whether a target type intersects a subset</summary>
    /// <param name="mask">The mask to compare</param>
    /// <param name="subset">The subset to compare</param>
    /// <returns>Returns true if the mask contains any bit of the subset, false if otherwise</returns>
    public static bool Intersects(this TargetExclusionState mask, TargetExclusionState subset) { return (mask & subset) != 0; }
  }
}
