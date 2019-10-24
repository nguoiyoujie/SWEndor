using SWEndor.Core;

namespace SWEndor.UI
{
  public partial class Font
  {
    public const string Consolas = "Consolas";
    public const string Impact = "Impact";

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

        Create(engine, T08, "Consolas_8");
        Create(engine, T10, "Consolas_10");
        Create(engine, T12, "Consolas_12");
        Create(engine, T14, "Consolas_14");
        Create(engine, T16, "Consolas_16");
        Create(engine, T24, "Consolas_24");
        Create(engine, T36, "Impact_36");
        Create(engine, T48, "Impact_48");

        /*
        Create(engine, T08, Consolas, 8, FontMode.NONE);
        Create(engine, T10, Consolas, 10, FontMode.BOLD);
        Create(engine, T12, Consolas, 12, FontMode.BOLD);
        Create(engine, T14, Consolas, 14, FontMode.BOLD);
        Create(engine, T16, Consolas, 16, FontMode.BOLD);
        Create(engine, T24, Consolas, 24, FontMode.BOLD);
        Create(engine, T36, Impact, 36, FontMode.NONE);
        Create(engine, T48, Impact, 48, FontMode.NONE);
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
