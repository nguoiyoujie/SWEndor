using System;
using SWEndor.Evaluator;
using System.IO;
using System.Collections.Generic;

namespace SWEndor.Scenarios.Scripting
{
  public class Script
  {
    private List<Expression> m_expr = new List<Expression>();

    public Script(string filepath, string scriptname)
    {
      if (!File.Exists(filepath))
        throw new FileNotFoundException("Script file '" + filepath + "' is not found!");

      Screen2D.Instance().LoadingTextLines.Add("loading script:" + scriptname);
      Screen2D.Instance().LoadingTextLines.RemoveAt(0);
      ScriptFactory.AddScript(scriptname, this);
    }

    public void AddExpression(string line)
    {
      Expression expr = new Expression(line);
      if (expr.Errors.Count > 0)
        throw new Exception(expr.Errors[0].Message);

      m_expr.Add(expr);
    }

    public void Run()
    {
      foreach (Expression expr in m_expr)
      {
        object val = expr.Eval();
        if (expr.Errors.Count > 0)
          throw new Exception(expr.Errors[0].Message);
      }
    }
  }
}
