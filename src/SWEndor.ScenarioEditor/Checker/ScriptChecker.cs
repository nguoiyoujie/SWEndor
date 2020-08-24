﻿using Primrose.Expressions;
using SWEndor.Game.Scenarios.Scripting;
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
        ScriptFile f = new ScriptFile(new Context(null));
        if (Log != null)
          f.ScriptReadBegin = (s) => { Log(s); };
        f.ReadFromFile(Path);
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
