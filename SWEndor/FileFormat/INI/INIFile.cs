using SWEndor.Primitives;
using System;
using System.IO;

namespace SWEndor.FileFormat.INI
{
  public partial class INIFile
  {
    public static string PreHeaderSectionName = "PRE-HEADER";

    public INIFile(string filepath)
    {
      FilePath = filepath;
      ReadFile();
    }

    public readonly string FilePath;
    private readonly ThreadSafeDictionary<string, INISection> Sections = new ThreadSafeDictionary<string, INISection>();
    public string[] SectionKeys { get { return Sections.GetKeys(); } }

    public void Reset()
    {
      Sections.Clear();
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
          Sections.Add(PreHeaderSectionName, currSection);

          while (!sr.EndOfStream)
          {
            string line = sr.ReadLine();

            if (INISection.INIHeaderLine.IsHeader(line))
            {
              currSection = new INISection(line);
              if (!Sections.ContainsKey(currSection.Header))
                Sections.Add(currSection.Header, currSection);
              else
                Sections[currSection.Header].Merge(currSection);
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
        foreach (INISection section in Sections.GetValues())
        {
          if (section != null)
          {
            if (section.Header != null && section.Header.Length > 0)
              sw.WriteLine(section.HLine.ToString());

            foreach (INISection.INILine line in section.Lines)
              sw.WriteLine(line.ToString());
          }
        }
      }
    }
  }
}
