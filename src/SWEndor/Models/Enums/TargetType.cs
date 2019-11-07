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

  public static class TargetTypeExt
  {
    public static bool Has(this TargetType src, TargetType flag)
    {
      return (src & flag) == flag;
    }

    public static bool Contains(this TargetType src, TargetType flag)
    {
      return (src & flag) != 0;
    }
  }
}
