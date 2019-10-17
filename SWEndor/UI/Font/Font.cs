using SWEndor.Core;
using System;

namespace SWEndor.UI
{
  public partial class Font
  {
    private Font(Engine engine
               , byte key
               , string fontname
               , int size = 10
               , FontMode mode = FontMode.NONE
               )
    {
      ID = engine.TrueVision.TVScreen2DText.TextureFont_Create(key.ToString()
                                                            , fontname
                                                            , size
                                                            , mode.Has(FontMode.BOLD)
                                                            , mode.Has(FontMode.UNDERLINED)
                                                            , mode.Has(FontMode.ITALIC)
                                                            , true);

      generateWidth(engine);
    }

    // we actually don't need to store all those setting variables as the created font cannot be changed
    public readonly int ID;

    public float GetWidth(string text)
    {
      float w = 0;
      for (int i = 0; i < text.Length; i++)
        w += width[text[i]];
      return w;
    }

    private float[] width;

    private void generateWidth(Engine engine)
    {
      width = new float[256];
      float dummy = 0;
      for (int b = 0; b <= byte.MaxValue; b++)
        engine.TrueVision.TVScreen2DText.TextureFont_GetTextSize(((char)b).ToString(), ID, ref width[b], ref dummy);
    }

    /*
    private char[] printables = new char[]
    {
      'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
      'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
      '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '-', '=', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '_', '+', '[', ']',
      '\\', '|', '{', '}', ';', '\'', ':', '\"', ',', '.', '/', '<', '>', '?', '`', '~', ' ', '\n', '\t'
    };
    */
  }

  [Flags]
  public enum FontMode : byte
  {
    NONE = 0,
    BOLD = 0x1,
    UNDERLINED = 0x2,
    ITALIC = 0x4,
  }

  public static class FontModeExt
  {
    public static bool Has(this FontMode src, FontMode flag)
    {
      return (src & flag) == flag;
    }
  }
}
