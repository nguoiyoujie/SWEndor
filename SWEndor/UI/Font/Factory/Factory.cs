using SWEndor.Primitives;

namespace SWEndor.UI
{
  public partial class Font
  {
    public static class Factory
    {
      private static ThreadSafeDictionary<string, Font> list = new ThreadSafeDictionary<string, Font>();
      private static Font Default = new Font("default", "Consolas", 10);

      public static Font Create(string key, string fontname, int size = 10, bool bold = false, bool underlined = false, bool italic = false, bool international = true)
      {
        Font font = new Font(key, fontname, size, bold, underlined, italic, international);
        list.Add(key, font);
        return font;
      }

      public static Font Get(string key)
      {
        Font ret = list.Get(key);
        if (ret == null)
          ret = Default;
        return ret;
      }
    }
  }
}
