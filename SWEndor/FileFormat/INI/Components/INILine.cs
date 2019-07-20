using SWEndor.Primitives;

namespace SWEndor.FileFormat.INI
{
  public partial class INIFile
  {
    public partial class INISection
    {
      public struct INILine
      {
        public string Key;
        public string Value;

        public bool HasKey { get { return Key != null && Key.Length > 0; } }

        public override string ToString()
        {
          return "{0}={1}".F(Key, Value);
        }

        public static INILine ReadLine(string line)
        {
          INILine ret = new INILine();
          ret.Reload(line);
          return ret;
        }

        private void Reload(string line)
        {
          if (line != null)
          {
            // Format: KEY=VALUE      ; or #COMMENT
            int compos1 = line.IndexOf(';');
            int compos2 = line.IndexOf('#');

            int compos = (compos1 == -1 || compos1 < compos2) ? compos2 : compos1;
            if (compos > -1)
              line = line.Substring(0, compos);

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
      }
    }
  }
}
