using Primrose.FileFormat.INI;
using Primrose.Expressions;
using SWEndor.Scenarios.Scripting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
        ScriptFile f = new ScriptFile(Path, new Context(null));
        if (Log != null)
          f.ScriptReadBegin = (s) => { Log(s); };
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
