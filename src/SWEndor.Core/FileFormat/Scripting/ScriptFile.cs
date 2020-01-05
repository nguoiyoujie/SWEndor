using Primrose.Expressions;
using Primrose.Primitives.Extensions;
using System;
using System.IO;
using System.Text;

namespace SWEndor.FileFormat.Scripting
{
  public delegate void ScriptReadDelegate(string name);

  public class ScriptFile
  {
    private static char[] seperator = new char[] { ':' };

    public ScriptFile(string filepath)
    {
      if (!File.Exists(filepath))
        throw new FileNotFoundException(TextLocalization.Get(TextLocalKeys.SCRIPT_NOTFOUND_ERROR).F(Path.GetFullPath(filepath)));

      FilePath = filepath;
    }

    public readonly string FilePath;
    public ScriptReadDelegate ScriptReadDelegate;

    public void ReadFile()
    {
      Script script = Script.Registry.Global;
      StringBuilder sb = new StringBuilder();
      int linenumber = 0;
      using (StreamReader sr = new StreamReader(FilePath))
      {
        while (!sr.EndOfStream)
        {
          string line = sr.ReadLine().Trim();

          if (line.EndsWith(":"))
          {
            if (script != null)
            {
              script.AddStatements(sb.ToString(), ref linenumber);
            }

            line = line.TrimEnd(seperator).Trim();

            ScriptReadDelegate?.Invoke(line);
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
          script.AddStatements(sb.ToString(), ref linenumber);
        }
      }
    }
  }
}
