using System;

namespace SWEndor.Game.Models
{
  /// <summary>
  /// Explode trigger conditions and actions
  /// </summary>
  [Flags]
  public enum ExplodeTrigger : ushort
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

    /// <summary>Attaches the explosion instance to the parent of the source object</summary>
    ATTACH_TO_PARENT = 0x10,

    /// <summary>Creates the explosion while the source object is normal</summary>
    WHILE_NORMAL = 0x100,

    /// <summary>Creates the explosion while the source object is dying</summary>
    WHILE_DYING = 0x200,

    /// <summary>Creates the explosion on the source object's death. This is usually triggered once</summary>
    ON_DEATH = 0x400,

    /// <summary>Creates the explosion while the source object is stunned</summary>
    WHILE_STUNNED = 0x800,
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