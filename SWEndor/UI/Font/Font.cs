using System;

namespace SWEndor.UI
{
  public partial class Font
  {
    [Flags]
    public enum FontMode : byte
    {
      NONE = 0,
      BOLD = 0x1,
      UNDERLINED = 0x2,
      ITALIC = 0x4,
    }

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
                                                            , mode.HasFlag(FontMode.BOLD)
                                                            , mode.HasFlag(FontMode.UNDERLINED)
                                                            , mode.HasFlag(FontMode.ITALIC)
                                                            , true);
    }

    // we actually don't need to store all those setting variables as the created font cannot be changed
    public readonly int ID;
  }
}
