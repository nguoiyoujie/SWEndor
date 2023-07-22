using System.Collections.Generic;

namespace SWEndor
{

  public static class ColorLocalization
  {
    static Dictionary<ColorLocalKeys, COLOR> keys = new Dictionary<ColorLocalKeys, COLOR>
    {
      { ColorLocalKeys.TRANSPARENT, new COLOR(0, 0, 0, 0) },
      { ColorLocalKeys.WHITE, new COLOR(1f, 1f, 1f, 1f) },

      { ColorLocalKeys.SYSTEM_FATAL, new COLOR(0.8f, 0.2f, 0.2f, 1f) },
      { ColorLocalKeys.SYSTEM_FATAL_BACKGROUND, new COLOR(0.8f, 0.2f, 0.2f, 0.3f) },
      { ColorLocalKeys.UI_BACKGROUND_DARK, new COLOR(0, 0, 0, 0.6f) },

      { ColorLocalKeys.UI_TEXT, new COLOR(0.8f, 0.8f, 0, 1f) },
      { ColorLocalKeys.UI_TEXT_ORANGE, new COLOR(1f, 0.5f, 0.2f, 1f) },
      { ColorLocalKeys.UI_TEXT_GOOD, new COLOR(0.2f, 0.8f, 0.2f, 1f) },
      { ColorLocalKeys.UI_TEXT_BAD, new COLOR(0.8f, 0.4f, 0.4f, 1f) },
      { ColorLocalKeys.UI_UNHIGHLIGHT_DARK_BACKGROUND, new COLOR(0, 0, 0, 0.925f) },
      { ColorLocalKeys.UI_UNHIGHLIGHT_BACKGROUND, new COLOR(0, 0, 0, 0.5f) },
      { ColorLocalKeys.UI_HIGHLIGHT_BACKGROUND,  new COLOR(0.05f, 0.2f, 0, 0.8f) },
      { ColorLocalKeys.UI_HIGHLIGHT_SELECT_BACKGROUND,  new COLOR(0.15f, 0.6f, 0.1f, 0.8f) },

      { ColorLocalKeys.GAME_LOAD_LIGHT, new COLOR(0.8f, 0.8f, 0, 1f) },
      { ColorLocalKeys.GAME_LOAD_DARK, new COLOR(0, 0, 0, 0.6f) },

      { ColorLocalKeys.GAME_MAP_GRID_MAJOR, new COLOR(1f, 1f, 0.2f, 0.6f) },
      { ColorLocalKeys.GAME_MAP_GRID_MINOR, new COLOR(1f, 1f, 0.2f, 0.3f) },
      { ColorLocalKeys.GAME_MAP_BACKGROUND, new COLOR(0, 0, 0, 0.8f) },

      { ColorLocalKeys.GAME_STAT_SPEED, new COLOR(0.7f, 0.8f, 0.4f, 1f) },
      { ColorLocalKeys.GAME_STAT_SHIELD, new COLOR(0.5f, 0.65f, 0.8f, 0.8f) },
      { ColorLocalKeys.GAME_STAT_CRITICAL_ALLY, new COLOR(0, 0.8f, 0.6f, 1f) },
      { ColorLocalKeys.GAME_STAT_CRITICAL_ENEMY, new COLOR(1f, 0, 0, 1f) },

      { ColorLocalKeys.GAME_MESSAGE_NORMAL, new COLOR(0.5f, 0.5f, 1f, 1f) },
      { ColorLocalKeys.GAME_MESSAGE_WARNING, new COLOR(1f, 0.2f, 0.2f, 1f) },
      { ColorLocalKeys.GAME_MESSAGE_BACKGROUND, new COLOR(0, 0, 0, 0.5f) },
      { ColorLocalKeys.GAME_MESSAGE_MISSILE_WARNING_1, new COLOR(1f, 0.2f, 0.2f, 1f) },
      { ColorLocalKeys.GAME_MESSAGE_MISSILE_WARNING_2, new COLOR(1f, 0.8f, 0.2f, 1f) },

      { ColorLocalKeys.GAME_SYSTEMSTATE_ACTIVE, new COLOR(0.3f, 1f, 0.3f, 1f) },
      { ColorLocalKeys.GAME_SYSTEMSTATE_ACTIVE_DAMAGED, new COLOR(0.8f, 0.8f, 0.2f, 1f) },
      { ColorLocalKeys.GAME_SYSTEMSTATE_DISABLED, new COLOR(0.2f, 0.2f, 0.6f, 1f) },
      { ColorLocalKeys.GAME_SYSTEMSTATE_DAMAGED, new COLOR(0.7f, 0.2f, 0.2f, 1f) },
      { ColorLocalKeys.GAME_SYSTEMSTATE_NULL, new COLOR(0.4f, 0.4f, 0.4f, 1f) },

      { ColorLocalKeys.GAME_SYSTEM_ION, new COLOR(0.6f, 0.6f, 1f, 1f) },
      { ColorLocalKeys.GAME_SYSTEM_DISABLED, new COLOR(0.4f, 0.5f, 1f, 0.4f) },
      { ColorLocalKeys.GAME_SYSTEM_DESTROYED, new COLOR(1f, 0, 0, 0.75f) },
    };

    public static COLOR Get(ColorLocalKeys key)
    {
      if (!keys.ContainsKey(key))
        return new COLOR();

      return keys[key];
    }
  }
}
