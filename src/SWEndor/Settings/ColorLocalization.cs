using MTV3D65;
using Primrose.Primitives.ValueTypes;
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
    static Dictionary<ColorLocalKeys, COLOR> keys = new Dictionary<ColorLocalKeys, COLOR>
    {
      { ColorLocalKeys.TRANSPARENT, new COLOR(0, 0, 0, 0) },
      { ColorLocalKeys.WHITE, new COLOR(1f, 1f, 1f, 1f) },

      { ColorLocalKeys.SYSTEM_FATAL, new COLOR(0.8f, 0.2f, 0.2f, 1f) },
      { ColorLocalKeys.SYSTEM_FATAL_BACKGROUND, new COLOR(0.8f, 0.2f, 0.2f, 0.3f) },
      { ColorLocalKeys.UI_BACKGROUND_DARK, new COLOR(0, 0, 0, 0.6f) },

      { ColorLocalKeys.UI_TEXT, new COLOR(0.8f, 0.8f, 0, 1f) },
      { ColorLocalKeys.UI_TEXT_ORANGE, new COLOR(1f, 0.5f, 0.2f, 1f) },
      { ColorLocalKeys.UI_UNHIGHLIGHT_BACKGROUND, new COLOR(0, 0, 0, 0.5f) },
      { ColorLocalKeys.UI_HIGHLIGHT_BACKGROUND,  new COLOR(0.05f, 0.2f, 0, 0.8f) },

      { ColorLocalKeys.GAME_LOAD_LIGHT, new COLOR(0.8f, 0.8f, 0, 1f) },
      { ColorLocalKeys.GAME_LOAD_DARK, new COLOR(0, 0, 0, 0.6f) },

      { ColorLocalKeys.GAME_MAP_GRID_MAJOR, new COLOR(1f, 1f, 0.2f, 0.6f) },
      { ColorLocalKeys.GAME_MAP_GRID_MINOR, new COLOR(1f, 1f, 0.2f, 0.3f) },
      { ColorLocalKeys.GAME_MAP_BACKGROUND, new COLOR(0, 0, 0, 0.8f) },

      { ColorLocalKeys.GAME_STAT_SPEED, new COLOR(0.7f, 0.8f, 0.4f, 1f) },
      { ColorLocalKeys.GAME_STAT_CRITICAL_ALLY, new COLOR(0, 0.8f, 0.6f, 1f) },
      { ColorLocalKeys.GAME_STAT_CRITICAL_ENEMY, new COLOR(1f, 0, 0, 1f) },

      { ColorLocalKeys.GAME_MESSAGE_NORMAL, new COLOR(0.5f, 0.5f, 1f, 1f) },
      { ColorLocalKeys.GAME_MESSAGE_WARNING, new COLOR(1f, 0.2f, 0.2f, 1f) },
      { ColorLocalKeys.GAME_MESSAGE_BACKGROUND, new COLOR(0, 0, 0, 0.5f) },
      { ColorLocalKeys.GAME_MESSAGE_MISSILE_WARNING_1, new COLOR(1f, 0.2f, 0.2f, 1f) },
      { ColorLocalKeys.GAME_MESSAGE_MISSILE_WARNING_2, new COLOR(1f, 0.8f, 0.2f, 1f) },

      { ColorLocalKeys.GAME_SYSTEMSTATE_ACTIVE, new COLOR(0.3f, 1f, 0.3f, 1f) },
      { ColorLocalKeys.GAME_SYSTEMSTATE_DISABLED, new COLOR(0.2f, 0.2f, 0.6f, 1f) },
      { ColorLocalKeys.GAME_SYSTEMSTATE_DESTROYED, new COLOR(0.7f, 0.2f, 0.2f, 1f) },
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

  public struct COLOR
  {
    public COLOR(float r, float g, float b, float a)
    {
      Value = 0;
      fA = a;
      fR = r;
      fG = g;
      fB = b;
    }

    public COLOR(float3 f)
    {
      Value = 0;
      fA = 1;
      fR = f[0];
      fG = f[1];
      fB = f[2];
    }

    public COLOR(float4 f)
    {
      Value = 0;
      fA = f[3];
      fR = f[0];
      fG = f[1];
      fB = f[2];
    }

    public int Value;

    public float fA
    {
      get { return ((Value & unchecked((int)0xFF000000)) >> 24) / 255f; }
      set { Value = Value & 0x00FFFFFF | (unchecked((int)(value * 255u)) << 24); }
    }

    public float fR
    {
      get { return ((Value & 0xFF0000) >> 16) / 255f; }
      set { Value = Value & unchecked((int)0xFF00FFFF) | (unchecked((int)(value * 255u)) << 16); }
    }

    public float fG
    {
      get { return ((Value & 0xFF00) >> 8) / 255f; }
      set { Value = Value & unchecked((int)0xFFFF00FF) | (unchecked((int)(value * 255u)) << 8); }
    }

    public float fB
    {
      get { return (Value & 0xFF) / 255f; }
      set { Value = Value & unchecked((int)0xFFFFFF00) | unchecked((int)(value * 255u)); }
    }

    public byte bA
    {
      get { return (byte)((Value & unchecked((int)0xFF000000)) >> 24); }
      set { Value = Value & 0x00FFFFFF | (value << 24); }
    }

    public byte bR
    {
      get { return (byte)((Value & 0xFF0000) >> 16); }
      set { Value = Value & unchecked((int)0xFF00FFFF) | (value << 16); }
    }

    public byte bG
    {
      get { return (byte)((Value & 0xFF00) >> 8); }
      set { Value = Value & unchecked((int)0xFFFF00FF) | (value << 8); }
    }

    public byte bB
    {
      get { return (byte)(Value & 0xFF); }
      set { Value = Value & unchecked((int)0xFFFFFF00) | value; }
    }
  }
}
