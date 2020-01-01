using System;

namespace SWEndor.Models
{
  /// <summary>
  /// The 'type' of an Actor for targeting purposes
  /// </summary>
  [Flags]
  public enum TargetType
  {
    NULL = 0,

    // Projectiles

    /// <summary>
    /// Actor is a laser
    /// </summary>
    LASER = 0x0001,

    /// <summary>
    /// Actor is a munition
    /// </summary>
    MUNITION = 0x0002,

    // Crafts

    /// <summary>
    /// Actor is a non-combat object
    /// </summary>
    FLOATING = 0x0010,

    /// <summary>
    /// Actor is a fighter
    /// </summary>
    FIGHTER = 0x0020,

    /// <summary>
    /// Actor is a large vessel
    /// </summary>
    SHIP = 0x0040,

    /// <summary>
    /// Actor is a 'structure' (think towers)
    /// </summary>
    STRUCTURE = 0x0080, // recheck, see if we can place it in a seperate category

    // Addons

    /// <summary>
    /// Actor is a dedicated add-on to another actor
    /// </summary>
    ADDON = 0x0100,

    /// <summary>
    /// Actor is a shield generator (special add on)
    /// </summary>
    SHIELDGENERATOR = 0x0200,

    /// <summary>
    /// Actor is considered a member of all targetable types.
    /// </summary>
    ANY = 0xFFFF
  }

  /// <summary>
  /// Provides extension methods for TargetType enum
  /// </summary>
  public static class TargetTypeExt
  {
    /// <summary>Returns whether a target type contains a subset</summary>
    /// <param name="mask">The mask to compare</param>
    /// <param name="subset">The subset to compare</param>
    /// <returns>Returns true if the mask contains all bits of the subset, false if otherwise</returns>
    public static bool Has(this TargetType mask, TargetType subset) { return (mask & subset) == subset; }

    /// <summary>Returns whether a target type intersects a subset</summary>
    /// <param name="mask">The mask to compare</param>
    /// <param name="subset">The subset to compare</param>
    /// <returns>Returns true if the mask contains any bit of the subset, false if otherwise</returns>
    public static bool Intersects(this TargetType mask, TargetType subset) { return (mask & subset) != 0; }
  }
}
