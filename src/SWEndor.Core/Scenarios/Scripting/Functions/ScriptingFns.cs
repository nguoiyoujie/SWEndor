using Primrose.Expressions;
using Primrose.Primitives.Extensions;
using SWEndor.Core;
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
    public static Val TryCall(IContext context, string script_name)
    {
      Engine e = ((Context)context).Engine;
      Script scr = e.ScriptContext.Scripts.Get(script_name);
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
    public static Val Call(IContext context, string script_name)
    {
      Engine e = ((Context)context).Engine;
      Script scr = e.ScriptContext.Scripts.Get(script_name);
      if (scr == null)
        throw new InvalidOperationException("Attempted to call non-existent script '{0}'".F(script_name));

      scr.Run(context);
      return Val.NULL;
    }
  }
}
