using System.Collections.Generic;

namespace SWEndor.Actors.Models
{
  public enum SystemPart : byte // up to 64 / 4 = 16 values
  {
    ENGINE, // Allow speed changes
    SIDE_THRUSTERS, // Allow steering 
    SHIELD_GENERATOR, // required to regen shields
    RADAR, // UI
    SCANNER, // UI, required to show target info
    TARGETING_SYSTEM, // UI, required for guided projectiles
    COMLINK, // allow communication with squad
    ENERGY_STORE, // stores 'energy'
    ENERGY_CHARGER, // 'energy' income
    LASER_WEAPONS, // required for laser projectiles
    PROJECTILE_LAUNCHERS, // required for guided projectiles
    HYPERDRIVE
  }

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

    public static string GetShorthand(this SystemPart part)
    {
      string s;
      if (!_shorthand.TryGetValue(part, out s))
      {
        s = part.ToString();
        if (s.Length > 3)
          s = s.Substring(0, 3);
      }
      return s;
    }
  }
}
