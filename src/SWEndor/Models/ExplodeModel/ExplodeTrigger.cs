using System;

namespace SWEndor.Models
{
  [Flags]
  public enum ExplodeTrigger : byte
  {
    NONE = 0,
    DONT_CREATE_ON_LOWFPS = 0x1,
    ONLY_WHEN_DYINGTIME_NOT_EXPIRED = 0x2,
    CREATE_ON_MESHVERTICES = 0x4,
    ATTACH_TO_ACTOR = 0x8,
    ON_NORMAL = 0x10,
    ON_DYING = 0x20,
    ON_DEATH = 0x40,
  }

  public static class ExplodeTriggerExt
  {
    public static bool Has(this ExplodeTrigger mask, ExplodeTrigger subset) { return (mask & subset) == subset; }
  }
}