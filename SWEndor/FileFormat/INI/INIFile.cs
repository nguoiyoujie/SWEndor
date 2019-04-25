using System;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.FileFormat.INI
{
  public partial class INIFile
  {
    public static string PreHeaderSectionName = "PRE-HEADER";

    public INIFile(string filepath)
    {
      m_filepath = filepath;
      ReadFile();
    }

    private string m_filepath = "";
    private Dictionary<string, INISection> m_section = new Dictionary<string, INISection>();

    public string FilePath { get { return m_filepath; } }
    public Dictionary<string, INISection> Sections { get { return m_section; } }

    public void Reset()
    {
      m_section.Clear();
    }

    public void ReadFile()
    {
      if (!File.Exists(FilePath))
      {
        throw new Exception(string.Format("The file {0} is not found!", FilePath));
      }
      else
      {
        Reset();

        using (StreamReader sr = new StreamReader(FilePath))
        {
          INISection currSection = new INISection("");
          m_section.Add(PreHeaderSectionName, currSection);

          while (!sr.EndOfStream)
          {
            string line = sr.ReadLine();

            if (INIHeaderLine.IsHeader(line))
            {
              currSection = new INISection(line);
              if (!m_section.ContainsKey(currSection.Header))
                m_section.Add(currSection.Header, currSection);
              else
                m_section[currSection.Header].Merge(currSection);
            }
            else
            {
              currSection.ReadLine(line);
            }
          }
        }
      }
    }

    public void SaveFile(string filepath)
    {
      Directory.CreateDirectory(Path.GetDirectoryName(filepath));

      using (StreamWriter sw = new StreamWriter(filepath, false))
      {
        foreach (INISection section in Sections.Values)
        {
          if (section != null)
          {
            if (section.Header != null && section.Header.Length > 0)
            {
              section.HeaderLine.UpdateRaw();
              sw.WriteLine(section.HeaderLine.Raw);
            }

            foreach (INILine line in section.Lines)
            {
              line.UpdateRaw();
              sw.WriteLine(line.Raw);
            }
          }
        }
      }
    }
  }
}
