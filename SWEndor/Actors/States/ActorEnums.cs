using System;

namespace SWEndor.Actors
{
  public enum CreationState { PLANNED, PREACTIVE, ACTIVE, DISPOSED }
  public enum ActorState { FIXED, FREE, HYPERSPACE, NORMAL, DYING, DEAD }
  public enum OrientationMode { ROTATION, DIRECTION }

  [Flags]
  public enum TargetType
  {
    NULL = 0,
    FIGHTER = 0x0001,
    SHIP = 0x0002,
    ADDON = 0x0004,
    SHIELDGENERATOR = 0x0008,
    STRUCTURE = SHIP | SHIELDGENERATOR,
    ANY = 0xFFFF
  }

  /*
  [Flags]
  public enum HuntBehaviour
  {
    RANDOM = 0,
    CHECKRANGE = 0x0001
  }
  */
}
