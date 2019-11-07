using System.Collections.Generic;

namespace SWEndor.FileFormat.INI
{
  public partial class INIFile
  {
    public partial class INISection
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
        return (Header == null || Header.Length == 0) ? PreHeaderSectionName : Header;
      }

      private INIHeaderLine m_headerline;
      private List<INILine> m_lines = new List<INILine>();

      internal string Header { get { return m_headerline.Header; } set { m_headerline.Header = value; } }
      internal string[] Inherits { get { return m_headerline.Inherits; } set { m_headerline.Inherits = value; } }
      internal INIHeaderLine HLine { get { return m_headerline; } }
      internal INILine[] Lines { get { return m_lines.ToArray(); } }

      internal void ReadLine(string line)
      {
        INILine newiniline = INILine.ReadLine(line);
        m_lines.Add(newiniline);
      }

      public string[] GetKeys()
      {
        string[] ret = new string[m_lines.Count];
        for (int i = 0; i < m_lines.Count; i++)
        {
          ret[i] = m_lines[i].Key;
        }
        return ret;
      }

      public string[] GetValues()
      {
        List<string> ret = new List<string>();
        for (int i = 0; i < m_lines.Count; i++)
        {
          if (m_lines[i].Value != null)
          {
            ret.Add(m_lines[i].Value);
          }
        }
        return ret.ToArray();
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
        throw new System.Exception("Key '" + key + "' not found in [" + Header + "]");
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
            m_lines[i] = new INILine { Key = key, Value = value };
            return;
          }
        }

        m_lines.Add(new INILine { Key = key, Value = value });
      }

      public void Merge(INISection section)
      {
        foreach (INILine line in section.Lines)
        {
          if (line.HasKey)
          {
            m_lines.Remove(GetLine(line.Key));
            m_lines.Add(line);
          }
        }
      }
    }
  }
}
