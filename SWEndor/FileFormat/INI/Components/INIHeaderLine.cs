using SWEndor.Primitives;
using System;

namespace SWEndor.FileFormat.INI
{
  public partial class INIFile
  {
    public partial class INISection
    {
      public class INIHeaderLine
      {
        public string Header;
        public string[] Inherits = new string[0];

        public override string ToString()
        {
          return (Inherits.Length > 0) ? "[{0}] : {1}".F(Header, string.Join(",", Inherits)) : "[{0}]".F(Header);
        }

        public static bool IsHeader(string line)
        {
          line = line.Trim();
          int startpos = line.IndexOf('[');
          int endpos = line.IndexOf(']');
          return (startpos == 0 && endpos > -1 && startpos < endpos);
        }

        public static INIHeaderLine ReadLine(string line)
        {
          INIHeaderLine ret = new INIHeaderLine();
          ret.Reload(line);
          return ret;
        }

        public void Reload(string line)
        {
          if (line != null)
          {
            // Format: [HEADER] : inheritHeader, inheritHeader, inheritHeader, ...      ;COMMENT
            int compos = line.IndexOf(';');
            if (compos > -1)
              line = line.Substring(0, compos);

            int inhpos = line.IndexOf(':');
            if (inhpos > -1)
            {
              if (line.Length > inhpos)
              {
                string[] headers = line.Substring(inhpos + 1).Split(INIFile.DefaultDelimiter, StringSplitOptions.RemoveEmptyEntries);
                Inherits = new string[headers.Length];
                for (int i = 0; i < headers.Length; i++)
                  Inherits[i] = headers[i].Trim();
              }
              line = line.Substring(0, inhpos);
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
      }
    }
  }
}
