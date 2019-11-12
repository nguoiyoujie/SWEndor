﻿using Primrose.Primitives.Extensions;
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
      FilePath = filepath;
      ReadFile();
    }

    public readonly string FilePath;
    private readonly Dictionary<string, INISection> m_sections = new Dictionary<string, INISection>();
    public Dictionary<string, INISection>.KeyCollection Sections { get { return m_sections.Keys; } }

    public void Reset()
    {
      m_sections.Clear();
    }

    public bool HasSection(string key)
    {
      if (m_sections.ContainsKey(key))
        return true;
      else
        return false;
    }

    public INISection GetSection(string key)
    {
      if (m_sections.ContainsKey(key))
        return m_sections[key];
      else
        throw new Exception("The section [{0}] does not exist in '{1}'!".F(key, FilePath));
    }

    public void ReadFile()
    {
      if (!File.Exists(FilePath))
      {
        throw new Exception("The file {0} is not found!".F(Path.GetFullPath(FilePath)));
      }
      else
      {
        Reset();

        using (StreamReader sr = new StreamReader(FilePath))
        {
          INISection currSection = new INISection("");
          m_sections.Add(PreHeaderSectionName, currSection);

          while (!sr.EndOfStream)
          {
            string line = sr.ReadLine();

            if (INISection.INIHeaderLine.IsHeader(line))
            {
              currSection = new INISection(line);
              if (!m_sections.ContainsKey(currSection.Header))
                m_sections.Add(currSection.Header, currSection);
              else
                m_sections[currSection.Header].Merge(currSection);
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
        foreach (INISection section in m_sections.Values)
        {
          if (section != null)
          {
            if (section.Header != null && section.Header.Length > 0)
              sw.WriteLine(section.HLine.ToString());

            foreach (INISection.INILine line in section.Lines)
            {
              string s = line.ToString();
              if (s.Length != 1) // "="
                sw.WriteLine(s);
            }

            sw.WriteLine();
          }
        }
      }
    }
  }
}