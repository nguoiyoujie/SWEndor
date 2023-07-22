using Primrose.Expressions;
using SWEndor.Game.Scenarios.Scripting;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SWEndor.ScenarioEditor.Checker
{
  public class ScriptChecker
  {
    public string Path;
    public RichTextBox Rtb;
    public Action<string> Log;
    public string Error;
    public ScriptChecker(string path, Action<string> log, RichTextBox rtb)
    {
      Path = path;
      Log = log;
      _readDelg = BeginScript;
      _processDelg = ProcessScript;
      Rtb = rtb;
    }

    public bool Verify()
    {
      try
      {
        ScriptFile f = new ScriptFile(new Context(null));
        if (Log != null)
        {
          f.ScriptReadBegin = _readDelg;
          f.ScriptReadEnd2 = _processDelg;
        }
        f.ReadFromFile(Path);
        return true;
      }
      catch (Exception ex)
      {
        Error = ex.Message + Environment.NewLine + ex.StackTrace;
        return false;
      }
    }

    private ScriptReadDelegate _readDelg;
    private ScriptReadDelegate2 _processDelg;

    public void BeginScript(string scriptname)
    {
      Rtb.SelectionColor = Color.Navy;
      Log(scriptname);
    }

    public void ProcessScript(Script script)
    {
      Rtb.SelectionColor = Color.SlateGray;
      Log(script.Write());
      Log("----------------------------------------------------------------");
    }
  }
}
