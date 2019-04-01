using System;

namespace SWEndor.ActorTypes
{
  [Flags]
  public enum TargetType
  {
    NULL = 0,
    FIGHTER = 0x0001,
    SHIP = 0x0002,
    ADDON = 0x0004,
    SHIELDGENERATOR = 0x0008,
    STRUCTURE = SHIP | SHIELDGENERATOR, // recheck, see if we can place it in a seperate category
    ANY = 0xFFFF
  }
}
