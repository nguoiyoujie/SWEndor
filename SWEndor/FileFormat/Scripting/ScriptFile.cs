using SWEndor.Scenarios.Scripting;
using System.IO;

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
      using (StreamReader sr = new StreamReader(FilePath))
      {
        while (!sr.EndOfStream)
        {
          string line = sr.ReadLine().Trim();

          if (line.EndsWith(":"))
          {
            line = line.TrimEnd(':').Trim();
            script = new Script(FilePath, line);
          }
          else
          {
            if (script != null)
            {
              if (line.Length > 0)
                script.AddExpression(line);
            }
          }
        }
      }
    }
  }
}
