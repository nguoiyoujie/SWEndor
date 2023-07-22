using Primrose.Primitives.Cache;
using System.Collections.Generic;

namespace SWEndor.Game.Actors.Models
{
  /// <summary>
  /// Defines subsystem parts
  /// </summary>
  public enum SystemPart : byte // up to 64 / 4 = 16 values
  {
    /// <summary>Allows speed changes</summary>
    ENGINE,

    /// <summary>Allows steering</summary>
    SIDE_THRUSTERS,

    /// <summary>Allows regeneration of shields</summary>
    SHIELD_GENERATOR,

    /// <summary>Reveals the in-game map and radar</summary>
    RADAR,

    /// <summary>Reveals information about the target</summary>
    SCANNER,

    /// <summary>Allows targeting, required for guided projectiles</summary>
    TARGETING_SYSTEM,

    /// <summary>Allows communication with squad and mission</summary>
    COMLINK,

    /// <summary>Allows accumulation of energy</summary>
    ENERGY_STORE,

    /// <summary>Converts fuel into energy</summary>
    ENERGY_CHARGER,

    /// <summary>Fires laser projectiles</summary>
    LASER_WEAPONS,

    /// <summary>Fires ion projectiles</summary>
    ION_WEAPONS,

    /// <summary>Launches ordinance projectiles</summary>
    PROJECTILE_LAUNCHERS,

    /// <summary>Allow hyperdrive (TO-DO)</summary>
    HYPERDRIVE
  }

  /// <summary>
  /// Provides extension methods for a SystemPart
  /// </summary>
  public static class SystemPartExt
  {
    private static Dictionary<SystemPart, string> _shorthand = new Dictionary<SystemPart, string>
    {
      // Try to limit to 3 characters
      { SystemPart.ENGINE, "ENG"},
      { SystemPart.SIDE_THRUSTERS, "THT"},
      { SystemPart.SHIELD_GENERATOR, "SHD"},
      { SystemPart.RADAR, "RDR"},
      { SystemPart.SCANNER, "SCN"},
      { SystemPart.TARGETING_SYSTEM, "TGT"},
      { SystemPart.COMLINK, "COM"},
      { SystemPart.ENERGY_STORE, "STO"},
      { SystemPart.ENERGY_CHARGER, "CHG"},
      { SystemPart.LASER_WEAPONS, "LSR"},
      { SystemPart.ION_WEAPONS, "ION"},
      { SystemPart.PROJECTILE_LAUNCHERS, "PRJ"},
      { SystemPart.HYPERDRIVE, "HYP"}
    };

    private static Dictionary<SystemPart, string> _displayName = new Dictionary<SystemPart, string>
    {
      // Try to limit to 16 characters
      { SystemPart.ENGINE, "ENGINE"},
      { SystemPart.SIDE_THRUSTERS, "SIDE THRUSTERS"},
      { SystemPart.SHIELD_GENERATOR, "SHIELD GENERATOR"},
      { SystemPart.RADAR, "RADAR"},
      { SystemPart.SCANNER, "SCANNER"},
      { SystemPart.TARGETING_SYSTEM, "TARGETING SYSTEM"},
      { SystemPart.COMLINK, "COMLINK"},
      { SystemPart.ENERGY_STORE, "ENERGY STORE"},
      { SystemPart.ENERGY_CHARGER, "ENERGY CHARGER"},
      { SystemPart.LASER_WEAPONS, "LASER WEAPONS"},
      { SystemPart.ION_WEAPONS, "ION_WEAPONS"},
      { SystemPart.PROJECTILE_LAUNCHERS, "WEAPON LAUNCHERS"},
      { SystemPart.HYPERDRIVE, "HYPERDRIVE"}
    };

    /// <summary>
    /// Retrieves the short name for a system part
    /// </summary>
    /// <param name="part">The system part</param>
    /// <returns>The cached short name of this part</returns>
    public static string GetShortName(this SystemPart part)
    {
      string s;
      if (!_shorthand.TryGetValue(part, out s))
      {
        s = Enum<SystemPart>.GetName(part);
        if (s.Length > 3)
          s = s.Substring(0, 3);
      }
      return s;
    }

    /// <summary>
    /// Retrieves the short name for a system part
    /// </summary>
    /// <param name="part">The system part</param>
    /// <returns>The cached short name of this part</returns>
    public static string GetDisplayName(this SystemPart part)
    {
      string s;
      if (!_displayName.TryGetValue(part, out s))
      {
        s = Enum<SystemPart>.GetName(part).Replace('_', ' ');
      }
      return s;
    }
  }
}
