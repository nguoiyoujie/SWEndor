using SWEndor.Primitives.Extensions;
using SWEndor.Scenarios.Scripting.Expressions;
using System;

namespace SWEndor.Scenarios.Scripting.Functions
{
  public static class ScriptingFns
  {
    /// <summary>
    /// Calls another script for execution
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="ps">
    ///   Parameters: 
    ///     STRING script_name
    /// </param>
    /// <returns>TRUE if a script is found, FALSE otherwise</returns>
    public static Val CallScript(Context context, Val[] ps)
    {
      Script scr = Script.Registry.Get((string)ps[0]);
      if (scr != null)
      {
        scr.Run(context);
        return Val.TRUE;
      }
      return Val.FALSE;
    }

    /// <summary>
    /// Calls another script for execution. This is the strict counterpart to CallScript.
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="ps">
    ///   Parameters: 
    ///     STRING script_name
    /// </param>
    /// <returns>NULL. Throws an exception if no script is found.</returns>
    public static Val CallScriptX(Context context, Val[] ps)
    {
      string name = (string)ps[0];
      Script scr = Script.Registry.Get(name);
      if (scr == null)
        throw new InvalidOperationException("Attempted to call non-existent script '{0}'".F(name));

      scr.Run(context);
      return Val.NULL;
    }
  }
}
