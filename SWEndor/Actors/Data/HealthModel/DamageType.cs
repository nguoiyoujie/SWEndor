using System;

namespace SWEndor.Actors
{
  [Flags]
  public enum DamageType
  {
    NONE = 0,
    ALWAYS_100PERCENT = 0x0001,
    NORMAL = 0x0010,
    COLLISION = 0x0100,

    ALL = 0xFFFF
  }

  public static class DamageTypeExt
  {
    public static bool Has(this DamageType mask, DamageType subset)
    {
      return (mask & subset) == subset;
    }
  }
}
