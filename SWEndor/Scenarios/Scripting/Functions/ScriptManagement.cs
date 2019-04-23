using SWEndor.Scenarios.Scripting.Expressions;
using System;

namespace SWEndor.Scenarios.Scripting.Functions
{
  public static class ScriptManagement
  {
    public static object CallScript(Context context, object[] ps)
    {
      Script scr = Script.Registry.Get(ps[0].ToString());
      if (scr != null)
      {
        scr.Run();
        return true;
      }
      return false;
    }

    public static object GetArrayElement(Context context, object[] ps)
    {
      if (!(ps[0] is Array))
        return null;

      return ((Array)ps[0]).GetValue(Convert.ToInt32(ps[1].ToString()));
    }

  }
}
