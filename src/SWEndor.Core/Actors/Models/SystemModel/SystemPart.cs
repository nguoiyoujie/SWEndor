﻿using Primrose.Primitives;
using System.Collections.Generic;

namespace SWEndor.Actors.Models
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
      { SystemPart.PROJECTILE_LAUNCHERS, "PRJ"},
      { SystemPart.HYPERDRIVE, "HYP"}
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
        s = part.GetEnumName();
        if (s.Length > 3)
          s = s.Substring(0, 3);
      }
      return s;
    }
  }
}
