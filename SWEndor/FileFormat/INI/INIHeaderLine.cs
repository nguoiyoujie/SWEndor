namespace SWEndor.FileFormat.INI
{
  public class INIHeaderLine
  {
    private const int CommentPosition = 40;

    private string m_raw;
    private string m_header;
    private string m_comment;

    public string Raw { get { return m_raw; } set { m_raw = value; } }
    public string Header { get { return m_header; } set { m_header = value; } }
    public string Comment { get { return m_comment; } set { m_comment = value; } }

    public static bool IsHeader(string line)
    {
      line = line.Trim();
      int startpos = line.IndexOf('[');
      int endpos = line.IndexOf(']');
      return (startpos == 0 && endpos > -1 && startpos < endpos);
      //int purecommentpos = line.IndexOf(";;");
      //return (purecommentpos == 0 || (startpos == 0 && endpos > -1 && startpos < endpos));
    }

    public static INIHeaderLine ReadLine(string line)
    {
      INIHeaderLine ret = new INIHeaderLine();
      ret.Raw = line;
      ret.Reload();
      return ret;
    }

    public void Reload()
    {
      string line = Raw;
      if (line != null)
      {
        // Format: [HEADER]      ;COMMENT
        int compos = line.IndexOf(';');
        if (compos > -1)
        {
          if (line.Length > compos)
            Comment = line.Substring(compos + 1);

          line = line.Substring(0, compos);
        }

        int startpos = line.IndexOf('[');
        int endpos = line.IndexOf(']');
        if (startpos > -1 && endpos > -1 && startpos < endpos)
        {
          Header = line.Substring(startpos + 1, endpos - startpos - 1);
        }
        else
          Header = "";
      }
    }

    public void UpdateRaw()
    {
      Raw = string.Format("[{0}]", Header);
      if (Comment != null && Comment.Length > 0)
        Raw = string.Format("{0," + CommentPosition + "};{1}", Raw, Comment);
    }
  }
}
