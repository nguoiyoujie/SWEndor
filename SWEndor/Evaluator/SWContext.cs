﻿using System.Collections.Generic;

namespace SWEndor.Evaluator
{
  public class SWContext
  {
    private static SWContext defaultContext;
    public static SWContext Default
    {
      get
      {
        if (defaultContext == null)
          defaultContext = new SWContext();
        return defaultContext;
      }
    }

    // list of functions currently in scope during an evaluation
    // note that this typically is NOT thread safe.

    // contains a list of variables that is in scope. Scope is used only for DynamicFunctions (for now)
    private List<Variables> inScope;

    public SWFunctions Functions { get; private set; }
    public Variables Globals { get; private set; }
    //public CommandPrompt Console { get; set; }
    /// <summary>
    /// check current stacksize
    /// is used for debugging purposes and error handling
    /// to prevent stackoverflows
    /// </summary>
    public int CurrentStackSize
    {
      get
      {
        return inScope.Count;
      }
    }

    public Variables CurrentScope
    {
      get
      {
        if (inScope.Count <= 0)
          return null;

        return inScope[inScope.Count - 1];
      }
    }

    public void PushScope(Variables vars)
    {
      inScope.Add(vars);
    }

    public Variables PopScope()
    {
      if (inScope.Count <= 0)
        return null;

      Variables vars = inScope[inScope.Count - 1];
      inScope.RemoveAt(inScope.Count - 1);
      return vars;
    }

    public SWContext()
    {
      Reset();
    }

    /// <summary>
    /// resets the context to its defaults
    /// </summary> 
    public void Reset()
    {
      inScope = new List<Variables>();
      Functions = new SWFunctions();
      Globals = new Variables();
      Functions.InitDefaults();
      Functions.InitSW();
      Globals["Pi"] = 3.1415926535897932384626433832795; // Math.Pi is not very precise
      Globals["E"] = 2.7182818284590452353602874713527;  // Math.E is not very precise either
    }
  }
}
