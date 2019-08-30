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
    }

    // we actually don't need to store all those setting variables as the created font cannot be changed
    public readonly int ID;
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
