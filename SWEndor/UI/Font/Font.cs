namespace SWEndor.UI
{
  public class Font
  {
    private static ThreadSafeDictionary<string, Font> list = new ThreadSafeDictionary<string, Font>();
    private static Font Default = new Font("default", "Consolas", 10);

    private Font(string key, string fontname, int size = 10, bool bold = false, bool underlined = false, bool italic = false, bool international = true)
    {
      Key = key;
      FontName = fontname;
      Size = size;
      Bold = bold;
      Underlined = underlined;
      Italic = italic;
      International = international;

      ID = Engine.Instance().TVScreen2DText.TextureFont_Create(Key, FontName, Size, Bold, Underlined, Italic, International);
      list.Add(Key, this);
    }

    public static Font CreateFont(string key, string fontname, int size = 10, bool bold = false, bool underlined = false, bool italic = false, bool international = true)
    {
      return new Font(key, fontname, size, bold, underlined, italic, international);
    }

    public static Font GetFont(string key)
    {
      Font ret = list.Get(key);
      if (ret == null)
        ret = Default;
      return ret;
    }

    public readonly int ID;
    public readonly string Key;
    public readonly string FontName;
    public readonly int Size;
    public readonly bool Bold;
    public readonly bool Underlined;
    public readonly bool Italic;
    public readonly bool International;
  }
}
