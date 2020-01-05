using Primrose.Expressions;
using SWEndor.FileFormat.Scripting;
using System;

namespace SWEndor.ScenarioEditor.Checker
{
  public class ScriptChecker
  {
    public string Path;
    public Action<string> Log;
    public string Error;
    public ScriptChecker(string path, Action<string> log)
    {
      Path = path;
      Log = log;
    }

    public bool Verify()
    {
      try
      {
        Script.Registry.Clear();
        ScriptFile f = new ScriptFile(Path);
        if (Log != null)
          f.ScriptReadDelegate = (s) => { Log(s); };
        f.ReadFile();
        return true;
      }
      catch (Exception ex)
      {
        Error = ex.Message + Environment.NewLine + ex.StackTrace;
        return false;
      }
    }
  }
}
