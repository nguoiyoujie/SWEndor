using Primrose.Expressions;
using Primrose.Primitives.Extensions;
using SWEndor.Game.Core;
using System;
using System.Collections.Generic;

namespace SWEndor.Game.Scenarios.Scripting.Functions
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
    /// <returns>the return value if a script is found, FALSE otherwise</returns>
    public static Val TryCall(IContext context, string script_name)
    {
      Engine e = ((Context)context).Engine;
      Script scr = e.ScriptContext.Scripts.Get(script_name);
      if (scr != null)
      {
        return scr.Run(context);
      }
      return Val.FALSE;
    }

    /// <summary>
    /// Calls another script for execution
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="ps">
    ///   Parameters: 
    ///     STRING script_name
    /// </param>
    /// <returns>the return value if a script is found, FALSE otherwise</returns>
    public static Val TryCall(IContext context, IList<Val> ps)
    {
      Engine e = ((Context)context).Engine;
      string script_name = ((string)ps[0]).ToLower();
      Script scr = e.ScriptContext.Scripts.Get(script_name);
      if (scr != null)
      {
        for (int i = 1; i < ps.Count; i++)
        {
          scr.Scope.SetParameter(i - 1, ps[i]);
        }
        return scr.Run(context);
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
    /// <returns>The return value if a script. Throws an exception if no script is found.</returns>
    public static Val Call(IContext context, string script_name)
    {
      Engine e = ((Context)context).Engine;
      Script scr = e.ScriptContext.Scripts.Get(script_name);
      if (scr == null)
        throw new InvalidOperationException("Attempted to call non-existent script '{0}'".F(script_name));

      return scr.Run(context);
    }

    /// <summary>
    /// Calls another script for execution. This is the strict counterpart to CallScript.
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="ps">
    ///   Parameters: 
    ///     STRING script_name
    ///     ... parameters
    /// </param>
    /// <returns>NULL. Throws an exception if no script is found.</returns>
    public static Val Call(IContext context, IList<Val> ps)
    {
      Engine e = ((Context)context).Engine;
      string script_name = (string)ps[0];
      Script scr = e.ScriptContext.Scripts.Get(script_name);
      if (scr == null)
        throw new InvalidOperationException("Attempted to call non-existent script '{0}'".F(script_name));

      int pcount = scr.Scope.ParameterCount;
      //if ((ps.Count - 1) > pcount)
      //  throw new InvalidOperationException("Script '{0}' can accept {1} parameters. Received {2}".F(script_name, pcount, ps.Count - 1));

      for (int i = 1; i < ps.Count; i++)
      {
        scr.Scope.SetParameter(i - 1, ps[i]);
      }

      return scr.Run(context);
    }
  }
}
