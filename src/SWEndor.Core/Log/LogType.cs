using System.Collections.Generic;

namespace SWEndor
{
  internal enum LogType
  {
    // System
    SYS_INIT,
    SYS_CLOSE,

    // Generic
    INFO,

    // Asset
    ASSET_LOADING,

    // Engine
    ENGINE_STARTED,
    ENGINE_ENDED,

    // Scenario
    SCENARIO_STARTED,
    SCENARIO_ENDED,
    SCENARIO_STATE_CHANGE,

    // Actor
#if DEBUG
    ACTOR_CREATED,
    ACTOR_CREATIONSTATECHANGED,
    ACTOR_ACTORSTATECHANGED,

    ACTOR_DAMAGED,
    ACTOR_HEALED,
    ACTOR_KILLED,
    ACTOR_KILLED_BY,
    ACTOR_DISPOSED,

    // AI
    ACTOR_AI_ACTION,
    ACTOR_AI_ACTION_COMPLETED,

    // Weapon
    WEAPON_FIRED,
#endif
  }

  internal static class LogDecorator
  {
    internal static readonly Dictionary<LogType, string> Decorator =
      new Dictionary<LogType, string>()
      {
        { LogType.SYS_INIT, "{0} initalizing, version {1}." },
        { LogType.SYS_CLOSE, "{0} exiting." },

        { LogType.INFO, "{0}" },

        { LogType.ASSET_LOADING, "Loading {0} {1}" },

#if DEBUG
        { LogType.ACTOR_CREATED, "{0} was created." },
        { LogType.ACTOR_CREATIONSTATECHANGED, "{0} creation state changed to {1}." },
        { LogType.ACTOR_ACTORSTATECHANGED, "{0} actor state changed to {1}." },


        { LogType.ACTOR_DAMAGED, "{0} was damaged by {1} for {2}. HP is now {3}." },
        { LogType.ACTOR_HEALED, "{0} was healed by {1} for {2}. HP is now {3}." },
        { LogType.ACTOR_KILLED, "{0} was killed." },
        { LogType.ACTOR_KILLED_BY, "{0} was killed by {1}." },
        { LogType.ACTOR_DISPOSED, "{0} was disposed." },
#endif



      };
  }
}
