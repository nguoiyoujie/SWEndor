using SWEndor.Game.Core;

namespace SWEndor.Game.UI
{
  public partial class Font
  {
    public const string Consolas = "Consolas";

    public const byte T08 = 8;
    public const byte T10 = 10;
    public const byte T12 = 12;
    public const byte T14 = 14;
    public const byte T16 = 16;
    public const byte T24 = 24;
    public const byte T36 = 36;
    public const byte T48 = 48;

    public class Factory
    {
      private Font Default;
      private Font[] list = new Font[byte.MaxValue];
      public void Init(Engine engine)
      {
        Default = new Font(engine, 0, Consolas, 10);

        Create(engine, T08, "Pixel_Operator_Mono_10");
        Create(engine, T10, "Pixel_Operator_Mono_12");
        Create(engine, T12, "Pixel_Operator_Mono_14");
        Create(engine, T14, "Pixel_Operator_Mono_16");
        Create(engine, T16, "Pixel_Operator_Mono_18");
        Create(engine, T24, "Pixel_Operator_Mono_24");
        Create(engine, T36, "Pixel_Operator_Mono_36");
        Create(engine, T48, "Pixel_Operator_Mono_48");
        
        /*
        Create(engine, T08, "Pixel Operator Mono", 10, FontMode.NONE); // 8
        Create(engine, T10, "Pixel Operator Mono", 12, FontMode.NONE); // 10
        Create(engine, T12, "Pixel Operator Mono", 14, FontMode.NONE); // 12
        Create(engine, T14, "Pixel Operator Mono", 16, FontMode.NONE); // 14
        Create(engine, T16, "Pixel Operator Mono", 18, FontMode.NONE); // 16
        Create(engine, T24, "Pixel Operator Mono", 24, FontMode.NONE);
        Create(engine, T36, "Pixel Operator Mono", 36, FontMode.BOLD);
        Create(engine, T48, "Pixel Operator Mono", 48, FontMode.BOLD);
        */
      }

      public Font Create(Engine engine, byte key, string fontname, int size = 10, FontMode mode = FontMode.NONE)
      {
        Font font = new Font(engine, key, fontname, size, mode);
        list[key] = font;
        return font;
      }

      public Font Create(Engine engine, byte key, string name)
      {
        Font font = new Font(engine, key, name);
        list[key] = font;
        return font;
      }

      public Font Get(byte key)
      {
        Font ret = list[key];
        if (ret == null)
          ret = Default;
        return ret;
      }
    }
  }
}
