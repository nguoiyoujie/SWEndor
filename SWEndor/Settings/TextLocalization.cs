using System.Collections.Generic;

namespace SWEndor
{
  public enum TextLocalKeys
  {
    DUMMY,

    // SYSTEM
    SYSTEM_INIT_ERROR,
    SYSTEM_RUN_ERROR,

    SYSTEM_TITLE_ERROR,
    SYSTEM_DISP_FATAL_ERROR,
    SYSTEM_TEXT_FATAL_ERROR,

    // SUB-SYSTEMS
    SOUND_INIT_ERROR = 1100,

    ACTOR_OVERFLOW_ERROR,
    ACTOR_INVALID_ERROR,
    EXPL_OVERFLOW_ERROR,
    EXPL_INVALID_ERROR,
    PROJ_OVERFLOW_ERROR,
    PROJ_INVALID_ERROR,

    ACTORTYPE_INVALID_ERROR,
    EXPLTYPE_INVALID_ERROR,
    PROJTYPE_INVALID_ERROR,

    WEAPON_NOTFOUND_ERROR,
    WEAPONLOAD_NOTFOUND_ERROR,
    SCRIPT_NOTFOUND_ERROR,

    // SCRIPTING
    SCRIPT_LEXER_INVALID,
    SCRIPT_PARSER_UNEXPECTED_TOKEN,
    SCRIPT_PARSER_UNEXPECTED_TOKEN_2,
    SCRIPT_EVAL_INVALID,
    SCRIPT_EVAL_INVALID_OP,
    SCRIPT_EVAL_INVALID_BOP,

    SCRIPT_UNEXPECTED_UOP,
    SCRIPT_UNEXPECTED_BOP,
    SCRIPT_UNEXPECTED_NONBOOL,

    // PLAYER FEEDBACK
    PLAYER_OUTOFBOUNDS,
    ENEMY_HIT,
    ENEMY_DESTROYED,
    SUBSYSTEM_LOST,
    SQUAD_MEMBER_LOST,

    SQUAD_INDICATOR,
    CAMERAMODE_SWITCH,
    TARGET_LOCKED,
    TARGET_UNLOCKED,
    MOVEMENT_LOCKED,
    MOVEMENT_UNLOCKED,
    TIME_MULT,

    // SQUAD COMMAND
    SQUAD_ATTACK_ACK,
    SQUAD_DEFEND_ACK,
    SQUAD_CLEAR_ACK,
    SQUAD_NOACK,
  }

  public static class TextLocalization
  {
    static Dictionary<TextLocalKeys, string> keys = new Dictionary<TextLocalKeys, string>
    {
      { TextLocalKeys.DUMMY, "" },

      { TextLocalKeys.SYSTEM_INIT_ERROR, "Fatal Error occurred during initialization. Please see {0} in the /Log folder for the error message." },
      { TextLocalKeys.SYSTEM_RUN_ERROR, "Fatal Error occurred during runtime. Please see {0} in the /Log folder for the error message." },

      { TextLocalKeys.SYSTEM_TITLE_ERROR, "{0} - Error Encountered!" },
      { TextLocalKeys.SYSTEM_DISP_FATAL_ERROR, "FATAL ERROR ENCOUNTERED" },
      { TextLocalKeys.SYSTEM_TEXT_FATAL_ERROR, "A fatal error has been encountered and the program needs to close.\nPlease see {0} in the /Log folder for the error message.\n\n" },

      { TextLocalKeys.SOUND_INIT_ERROR, "FMOD Sound failed to initailize./n/nResult Code: {0}" },

      { TextLocalKeys.ACTOR_OVERFLOW_ERROR, "Number of current actors exceeded limit of {0}!" },
      { TextLocalKeys.ACTOR_INVALID_ERROR, "Attempted to register actor with null ActorType!" },
      { TextLocalKeys.EXPL_OVERFLOW_ERROR, "Number of current explosions exceeded limit of {0}!" },
      { TextLocalKeys.EXPL_INVALID_ERROR, "Attempted to register explosion with null ExplosionType!" },
      { TextLocalKeys.PROJ_OVERFLOW_ERROR, "Number of current projectiles exceeded limit of {0}!" },
      { TextLocalKeys.PROJ_INVALID_ERROR, "Attempted to register projectile with null ProjectileType!" },

      { TextLocalKeys.ACTORTYPE_INVALID_ERROR, "ActorType '{0}' does not exist" },
      { TextLocalKeys.EXPLTYPE_INVALID_ERROR, "ExplosionType '{0}' does not exist" },
      { TextLocalKeys.PROJTYPE_INVALID_ERROR, "ProjectileType '{0}' does not exist" },

      { TextLocalKeys.WEAPON_NOTFOUND_ERROR, "Weapon '{0}' is not found!" },
      { TextLocalKeys.WEAPONLOAD_NOTFOUND_ERROR, "Weapon loadout '{0}' is not found!" },
      { TextLocalKeys.SCRIPT_NOTFOUND_ERROR, "Script file '{0}' is not found!" },

      { TextLocalKeys.SCRIPT_LEXER_INVALID, "Unable to match against any tokens at line {0} position {1} \"{2}\"" },
      { TextLocalKeys.SCRIPT_PARSER_UNEXPECTED_TOKEN, "Unexpected token '{0}' found at line {1}:{2}.\nLine: {3}" },
      { TextLocalKeys.SCRIPT_PARSER_UNEXPECTED_TOKEN_2, "Unexpected token '{0}' found at line {1}:{2}. Expected: {3}.\nLine: {4}" },
      { TextLocalKeys.SCRIPT_EVAL_INVALID, "Unable to execute script at line {0}:{1} \nReason: {2} \n" },
      { TextLocalKeys.SCRIPT_EVAL_INVALID_OP, "Unable to perform '{0}' operation on {1} at line {2}:{3} \nReason: {4}" },
      { TextLocalKeys.SCRIPT_EVAL_INVALID_BOP, "Unable to perform '{0}' operation between {1} and {2} at line {3}:{4} \nReason: {5}" },

      { TextLocalKeys.SCRIPT_UNEXPECTED_UOP, "Operation '{0}' incompatible with {1}" },
      { TextLocalKeys.SCRIPT_UNEXPECTED_BOP, "Operation '{0}' incompatible between {1} and {2}" },
      { TextLocalKeys.SCRIPT_UNEXPECTED_NONBOOL, "Non-boolean value {0} found at start of conditional expression" },

      

      { TextLocalKeys.PLAYER_OUTOFBOUNDS, "You are going out of bounds! Return to the battle!" },

      { TextLocalKeys.ENEMY_HIT, "[HIT]" },
      { TextLocalKeys.ENEMY_DESTROYED, "[{0}] destroyed." },
      { TextLocalKeys.SUBSYSTEM_LOST, "WARNING: Subsystem [{0}] lost." },
      { TextLocalKeys.SQUAD_MEMBER_LOST, "WARNING: [{0}] lost." },

      { TextLocalKeys.SQUAD_INDICATOR, "Squad Indicator: {0}" },
      { TextLocalKeys.CAMERAMODE_SWITCH, "CAMERAMODE: {0}" },
      { TextLocalKeys.TARGET_LOCKED, "Target Locked to {0}" },
      { TextLocalKeys.TARGET_UNLOCKED, "Target Unlocked" },
      { TextLocalKeys.MOVEMENT_LOCKED, "Movement Locked" },
      { TextLocalKeys.MOVEMENT_UNLOCKED, "Movement Unlocked" },

      { TextLocalKeys.TIME_MULT, "Game speed set to x{0:0.00}" },

      { TextLocalKeys.SQUAD_ATTACK_ACK, "Directing squad to attack {0}." },
      { TextLocalKeys.SQUAD_DEFEND_ACK, "Directing squad to assist {0}." },
      { TextLocalKeys.SQUAD_CLEAR_ACK, "Discarding squad orders." },
      { TextLocalKeys.SQUAD_NOACK, "You are not the squad leader. You cannot issue commands to your squad!" },




    };

    public static string Get(TextLocalKeys key)
    {
      if (!keys.ContainsKey(key))
        return "";

      return keys[key];
    }
  }
}
