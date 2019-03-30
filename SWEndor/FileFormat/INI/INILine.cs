namespace SWEndor.FileFormat.INI
{
  public class INILine
  {
    private const int CommentPosition = 40;

    private string m_raw;
    private string m_key;
    private string m_value;
    private string m_comment;

    public string Raw { get { return m_raw; } set { m_raw = value; } }
    public string Key { get { return m_key; } set { m_key = value; } }
    public string Value { get { return m_value; } set { m_value = value; } }
    public string Comment { get { return m_comment; } set { m_comment = value; } }

    public bool HasKey { get { return m_key != null && m_key.Length > 0; } }

    public override string ToString()
    {
      return Raw;
    }

    public static INILine ReadLine(string line)
    {
      INILine ret = new INILine();
      ret.Raw = line;
      ret.Reload();
      return ret;
    }

    public void Reload()
    {
      string line = Raw;
      if (line != null)
      {
        // Format: KEY=VALUE      ; or #COMMENT
        int compos1 = line.IndexOf(';');
        int compos2 = line.IndexOf('#');

        int compos = (compos1 == -1 || compos1 < compos2) ? compos2 : compos1;
        if (compos > -1)
        {
          if (line.Length > compos)
            Comment = line.Substring(compos + 1);

          line = line.Substring(0, compos);
        }

        int keypos = line.IndexOf('=');
        if (keypos > -1)
        {
          if (line.Length > keypos)
            Value = line.Substring(keypos + 1).Trim();

          Key = line.Substring(0, keypos).Trim();
        }
        else
          Key = line;
      }
    }

    public void UpdateRaw()
    {
      Raw = "";
      if (HasKey)
        Raw = string.Format("{0}={1}", Key, Value);
      if (Comment != null && Comment.Length > 0)
        Raw = string.Format("{0,-" + CommentPosition + "};{1}", Raw, Comment);
    }
  }
}
