using SWEndor.Scenarios.Scripting;
using System;
using System.IO;
using System.Text;

namespace SWEndor.FileFormat.Scripting
{
  public class ScriptFile
  {
    public ScriptFile(string filepath)
    {
      if (!File.Exists(filepath))
        throw new FileNotFoundException("Script file '" + filepath + "' is not found!");

      FilePath = filepath;
      ReadFile();
    }

    public readonly string FilePath;

    public void ReadFile()
    {
      Script script = null;
      StringBuilder sb = new StringBuilder();
      using (StreamReader sr = new StreamReader(FilePath))
      {
        while (!sr.EndOfStream)
        {
          string line = sr.ReadLine().Trim();

          if (line.EndsWith(":"))
          {
            if (script != null)
            {
              script.AddExpression(sb.ToString());
            }

            line = line.TrimEnd(':').Trim();
            script = new Script(FilePath, line);
            sb.Clear();
          }
          else
          {
            if (script != null)
            {
              sb.Append(line);
              sb.Append(Environment.NewLine);
            }
          }
        }
        if (script != null) // last script
        {
          script.AddExpression(sb.ToString());
        }
      }
    }
  }
}
