using System;

namespace SWEndor.Models
{
  /// <summary>
  /// Explode trigger conditions and actions
  /// </summary>
  [Flags]
  public enum ExplodeTrigger : byte
  {
    /// <summary>The trigger has no effect</summary>
    NONE = 0,

    /// <summary>Forbids creation of explosion when the low FPS flag is on</summary>
    DONT_CREATE_ON_LOWFPS = 0x1,

    /// <summary>Forbids creation of explosionif the dying time is expired</summary>
    ONLY_WHEN_DYINGTIME_NOT_EXPIRED = 0x2,

    /// <summary>Creates the explosion on one of the source object's mesh vertex</summary>
    CREATE_ON_MESHVERTICES = 0x4,

    /// <summary>Attaches the explosion instance to the source object</summary>
    ATTACH_TO_ACTOR = 0x8,

    /// <summary>Creates the explosion while the source object is normal</summary>
    ON_NORMAL = 0x10,

    /// <summary>Creates the explosion while the source object is dying</summary>
    ON_DYING = 0x20,

    /// <summary>Creates the explosion on the source object's death</summary>
    ON_DEATH = 0x40,
  }

  /// <summary>
  /// Provides extension methods for ExplodeTrigger enum
  /// </summary>
  public static class ExplodeTriggerExt
  {
    /// <summary>Returns whether a target type intersects a subset</summary>
    /// <param name="mask">The mask to compare</param>
    /// <param name="subset">The subset to compare</param>
    /// <returns>Returns true if the mask contains any bit of the subset, false if otherwise</returns>
    public static bool Has(this ExplodeTrigger mask, ExplodeTrigger subset) { return (mask & subset) == subset; }
  }
}