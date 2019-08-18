using System.Collections.Generic;

namespace SWEndor
{
  internal enum LogType
  {
    // System
    SYS_INIT,
    SYS_CLOSE,

    // Asset
    ASSET_TEX_LOAD,
    ASSET_TEX_LOADED,
    ASSET_MODEL_LOAD,
    ASSET_MODEL_LOADED,
    ASSET_SOUND_LOAD,
    ASSET_SOUND_LOADED,

    // Engine
    ENGINE_STARTED,
    ENGINE_ENDED,

    // Scenario
    SCENARIO_STARTED,
    SCENARIO_ENDED,
    SCENARIO_STATE_CHANGE,

    // Actor
    ACTOR_CREATED,
    ACTOR_DAMAGED,
    ACTOR_KILLED,
    ACTOR_KILLED_BY,
    ACTOR_DISPOSED,

    // AI
    ACTOR_AI_ACTION,
    ACTOR_AI_ACTION_COMPLETED,

    // Weapon
    WEAPON_FIRED,
  }

  internal static class LogDecorator
  {
    internal static readonly Dictionary<LogType, string> Decorator =
      new Dictionary<LogType, string>()
      {
        { LogType.SYS_INIT, "{0} initalizing, version {1}." },
        { LogType.SYS_CLOSE, "{0} exiting." },

        { LogType.ASSET_TEX_LOAD, "{0} loading..." },
        { LogType.ASSET_TEX_LOADED, "{0} loaded." },
        { LogType.ASSET_MODEL_LOAD, "{0} loading..." },
        { LogType.ASSET_MODEL_LOADED, "{0} loaded." },
        { LogType.ASSET_SOUND_LOAD, "{0} loading..." },
        { LogType.ASSET_SOUND_LOADED, "{0} loaded." },

        { LogType.ACTOR_CREATED, "{0} was created." },
        { LogType.ACTOR_DAMAGED, "{0} was damaged by {1} for {2}." },
        { LogType.ACTOR_KILLED, "{0} was killed." },
        { LogType.ACTOR_KILLED_BY, "{0} was killed by {1}." },
        { LogType.ACTOR_DISPOSED, "{0} was disposed." },




      };

  }
}
