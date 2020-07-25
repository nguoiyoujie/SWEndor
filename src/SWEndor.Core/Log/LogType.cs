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
}
