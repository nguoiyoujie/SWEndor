using System.Collections.Generic;

namespace SWEndor.FileFormat.INI
{
  public class INISection
  {
    public INISection(string headerline)
    {
      if (headerline == null)
        m_headerline = new INIHeaderLine();
      else
        m_headerline = INIHeaderLine.ReadLine(headerline);
    }

    public override string ToString()
    {
      return (Header == null || Header.Length == 0) ? INIFile.PreHeaderSectionName : Header;
    }

    private INIHeaderLine m_headerline;
    private List<INILine> m_lines = new List<INILine>();

    public string Header { get { return m_headerline.Header; } set { m_headerline.Header = value; } }
    public INIHeaderLine HeaderLine { get { return m_headerline; } }
    public List<INILine> Lines { get { return m_lines; } }

    public void ReadLine(string line)
    {
      INILine newiniline = INILine.ReadLine(line);
      m_lines.Add(newiniline);
    }

    public bool HasKey(string key)
    {
      for (int i = 0; i < m_lines.Count; i++)
      {
        if (m_lines[i].Key == key)
          return true;
      }
      return false;
    }

    public INILine GetLine(string key)
    {
      for (int i = 0; i < m_lines.Count; i++)
      {
        if (m_lines[i].Key == key)
          return m_lines[i];
      }

      return null;
    }

    public string GetValue(string key, string defaultvalue = "")
    {
      for (int i = 0; i < m_lines.Count; i++)
      {
        if (m_lines[i].Key == key)
          return m_lines[i].Value;
      }

      return defaultvalue;
    }

    public void SetValue(string key, string value)
    {
      for (int i = 0; i < m_lines.Count; i++)
      {
        if (m_lines[i].Key == key)
        {
          m_lines[i].Value = value;
          return;
        }
      }

      m_lines.Add(new INILine { Key = key, Value = value });
    }

    public string GetComment(string key)
    {
      for (int i = 0; i < m_lines.Count; i++)
      {
        if (m_lines[i].Key == key)
          return m_lines[i].Comment;
      }

      return "";
    }

    public void SetComment(string key, string comment)
    {
      for (int i = 0; i < m_lines.Count; i++)
      {
        if (m_lines[i].Key == key)
        {
          m_lines[i].Comment = comment;
          return;
        }
      }

      m_lines.Add(new INILine { Key = key, Comment = comment });
    }

    public void Merge(INISection section)
    {
      foreach (INILine line in section.Lines)
      {
        if (line.HasKey)
        {
          if (GetLine(line.Key) == null)
            m_lines.Add(line);
          else
          {
            m_lines.Remove(GetLine(line.Key));
            m_lines.Add(line);
          }
        }
      }
    }
  }
}
