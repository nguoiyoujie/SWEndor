using MTV3D65;
using System.Collections.Generic;

namespace SWEndor
{
  public enum ColorLocalKeys
  {
    TRANSPARENT,
    WHITE,

    SYSTEM_FATAL,
    SYSTEM_FATAL_BACKGROUND,
    UI_BACKGROUND_DARK,

    UI_TEXT,
    UI_TEXT_ORANGE,
    UI_UNHIGHLIGHT_BACKGROUND,
    UI_HIGHLIGHT_BACKGROUND,

    GAME_LOAD_LIGHT,
    GAME_LOAD_DARK,

    GAME_MAP_GRID_MAJOR,
    GAME_MAP_GRID_MINOR,
    GAME_MAP_BACKGROUND,

    GAME_STAT_SPEED,
    GAME_STAT_CRITICAL_ALLY,
    GAME_STAT_CRITICAL_ENEMY,

    GAME_MESSAGE_NORMAL,
    GAME_MESSAGE_WARNING,
    GAME_MESSAGE_BACKGROUND,
    GAME_MESSAGE_MISSILE_WARNING_1,
    GAME_MESSAGE_MISSILE_WARNING_2,

    GAME_SYSTEMSTATE_ACTIVE,
    GAME_SYSTEMSTATE_DISABLED,
    GAME_SYSTEMSTATE_DESTROYED,
    GAME_SYSTEMSTATE_NULL,

    GAME_SYSTEM_ION,
    GAME_SYSTEM_DISABLED,
    GAME_SYSTEM_DESTROYED,

  }

  public static class ColorLocalization
  {
    static Dictionary<ColorLocalKeys, int> keys = new Dictionary<ColorLocalKeys, int>
    {
      { ColorLocalKeys.TRANSPARENT, new TV_COLOR(0, 0, 0, 0).GetIntColor() },
      { ColorLocalKeys.WHITE, new TV_COLOR(1, 1, 1, 1).GetIntColor() },

      { ColorLocalKeys.SYSTEM_FATAL, new TV_COLOR(0.8f, 0.2f, 0.2f, 1).GetIntColor() },
      { ColorLocalKeys.SYSTEM_FATAL_BACKGROUND, new TV_COLOR(0.8f, 0.2f, 0.2f, 0.3f).GetIntColor() },
      { ColorLocalKeys.UI_BACKGROUND_DARK, new TV_COLOR(0, 0, 0, 0.6f).GetIntColor() },

      { ColorLocalKeys.UI_TEXT, new TV_COLOR(0.8f, 0.8f, 0, 1).GetIntColor() },
      { ColorLocalKeys.UI_TEXT_ORANGE, new TV_COLOR(1, 0.5f, 0.2f, 1).GetIntColor() },
      { ColorLocalKeys.UI_UNHIGHLIGHT_BACKGROUND, new TV_COLOR(0, 0, 0, 0.5f).GetIntColor() },
      { ColorLocalKeys.UI_HIGHLIGHT_BACKGROUND,  new TV_COLOR(0.05f, 0.2f, 0, 0.8f).GetIntColor() },

      { ColorLocalKeys.GAME_LOAD_LIGHT, new TV_COLOR(0.8f, 0.8f, 0, 1).GetIntColor() },
      { ColorLocalKeys.GAME_LOAD_DARK, new TV_COLOR(0, 0, 0, 0.6f).GetIntColor() },

      { ColorLocalKeys.GAME_MAP_GRID_MAJOR, new TV_COLOR(1, 1, 0.2f, 0.6f).GetIntColor() },
      { ColorLocalKeys.GAME_MAP_GRID_MINOR, new TV_COLOR(1, 1, 0.2f, 0.3f).GetIntColor() },
      { ColorLocalKeys.GAME_MAP_BACKGROUND, new TV_COLOR(0, 0, 0, 0.8f).GetIntColor() },

      { ColorLocalKeys.GAME_STAT_SPEED, new TV_COLOR(0.7f, 0.8f, 0.4f, 1).GetIntColor() },
      { ColorLocalKeys.GAME_STAT_CRITICAL_ALLY, new TV_COLOR(0, 0.8f, 0.6f, 1).GetIntColor() },
      { ColorLocalKeys.GAME_STAT_CRITICAL_ENEMY, new TV_COLOR(1, 0, 0, 1).GetIntColor() },

      { ColorLocalKeys.GAME_MESSAGE_NORMAL, new TV_COLOR(0.5f, 0.5f, 1, 1).GetIntColor() },
      { ColorLocalKeys.GAME_MESSAGE_WARNING, new TV_COLOR(1, 0.2f, 0.2f, 1).GetIntColor() },
      { ColorLocalKeys.GAME_MESSAGE_BACKGROUND, new TV_COLOR(0, 0, 0, 0.5f).GetIntColor() },
      { ColorLocalKeys.GAME_MESSAGE_MISSILE_WARNING_1, new TV_COLOR(1, 0.2f, 0.2f, 1).GetIntColor() },
      { ColorLocalKeys.GAME_MESSAGE_MISSILE_WARNING_2, new TV_COLOR(1, 0.8f, 0.2f, 1).GetIntColor() },

      { ColorLocalKeys.GAME_SYSTEMSTATE_ACTIVE, new TV_COLOR(0.3f, 1, 0.3f, 1).GetIntColor() },
      { ColorLocalKeys.GAME_SYSTEMSTATE_DISABLED, new TV_COLOR(0.2f, 0.2f, 0.6f, 1).GetIntColor() },
      { ColorLocalKeys.GAME_SYSTEMSTATE_DESTROYED, new TV_COLOR(0.7f, 0.2f, 0.2f, 1).GetIntColor() },
      { ColorLocalKeys.GAME_SYSTEMSTATE_NULL, new TV_COLOR(0.4f, 0.4f, 0.4f, 1).GetIntColor() },

      { ColorLocalKeys.GAME_SYSTEM_ION, new TV_COLOR(0.6f, 0.6f, 1, 1).GetIntColor() },
      { ColorLocalKeys.GAME_SYSTEM_DISABLED, new TV_COLOR(0.4f, 0.5f, 1, 0.4f).GetIntColor() },
      { ColorLocalKeys.GAME_SYSTEM_DESTROYED, new TV_COLOR(1, 0, 0, 0.75f).GetIntColor() },
    };

    public static int Get(ColorLocalKeys key)
    {
      if (!keys.ContainsKey(key))
        return 0;

      return keys[key];
    }

    // TV_COLOR is 0xAARRGGBB
    public static float GetA(this int color) { return ((color & unchecked((int)0xFF000000)) >> 24) / 255f; }

    public static float GetR(this int color) { return ((color & 0xFF0000) >> 16) / 255f; }

    public static float GetG(this int color) { return ((color & 0xFF00) >> 8) / 255f; }

    public static float GetB(this int color) { return (color & 0xFF) / 255f; }

    public static int SetA(this int color, byte a) { return color & 0x00FFFFFF | (a << 24); }

    public static int SetR(this int color, byte r) { return color & unchecked((int)0xFF00FFFF) | (r << 16); }

    public static int SetG(this int color, byte g) { return color & unchecked((int)0xFFFF00FF) | (g << 8); }

    public static int SetB(this int color, byte b) { return color & unchecked((int)0xFFFFFF00) | b; }

    public static int SetA(this int color, float a) { return color & 0x00FFFFFF | (unchecked((int)(a * 255u)) << 24); }

    public static int SetR(this int color, float r) { return color & unchecked((int)0xFF00FFFF) | (unchecked((int)(r * 255u)) << 16); }

    public static int SetG(this int color, float g) { return color & unchecked((int)0xFFFF00FF) | (unchecked((int)(g * 255u)) << 8); }

    public static int SetB(this int color, float b) { return color & unchecked((int)0xFFFFFF00) | unchecked((int)(b * 255u)); }
  }
}
