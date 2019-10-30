using SWEndor.Primitives.Extensions;
using SWEndor.Scenarios.Scripting;
using System;
using System.IO;
using System.Text;

namespace SWEndor.FileFormat.Scripting
{
  public class ScriptFile
  {
    private static char[] seperator = new char[] { ':' };

    public ScriptFile(string filepath)
    {
      if (!File.Exists(filepath))
        throw new FileNotFoundException(TextLocalization.Get(TextLocalKeys.SCRIPT_NOTFOUND_ERROR).F(Path.GetFullPath(filepath)));

      FilePath = filepath;
      ReadFile();
    }

    public readonly string FilePath;

    public void ReadFile()
    {
      Script script = Script.Registry.Global;
      StringBuilder sb = new StringBuilder();
      int linenumber = 1;
      using (StreamReader sr = new StreamReader(FilePath))
      {
        while (!sr.EndOfStream)
        {
          string line = sr.ReadLine().Trim();

          if (line.EndsWith(":"))
          {
            if (script != null)
            {
              script.AddExpression(sb.ToString(), ref linenumber);
            }

            line = line.TrimEnd(seperator).Trim();
            script = new Script(line);
            sb.Clear();
          }
          else
          {
            if (script != null)
            {
              sb.Append(line);
              sb.Append(Environment.NewLine);
            }
            else // Globals
            {
              
            }
          }
        }
        if (script != null) // last script
        {
          script.AddExpression(sb.ToString(), ref linenumber);
        }
      }
    }
  }
}
