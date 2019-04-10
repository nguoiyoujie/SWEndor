using System;

namespace SWEndor.ActorTypes
{
  /// <summary>
  /// The 'type' of an Actor for targeting purposes
  /// </summary>
  [Flags]
  public enum TargetType
  {
    NULL = 0,

    /// <summary>
    /// Actor is a fighter
    /// </summary>
    FIGHTER = 0x0001,

    /// <summary>
    /// Actor is a large vessel
    /// </summary>
    SHIP = 0x0002,

    /// <summary>
    /// Actor is a dedicated add-on to another actor
    /// </summary>
    ADDON = 0x0004,

    /// <summary>
    /// Actor is a shield generator
    /// </summary>
    SHIELDGENERATOR = 0x0008,

    /// <summary>
    /// Actor is a 'structure' (think towers)
    /// </summary>
    STRUCTURE = SHIP | SHIELDGENERATOR, // recheck, see if we can place it in a seperate category

    /// <summary>
    /// Actor is considered a member of all targetable types.
    /// </summary>
    ANY = 0xFFFF
  }
}
